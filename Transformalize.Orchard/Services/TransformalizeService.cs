using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.AppData;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Reports;
using Orchard.Reports.Services;
using Orchard.Utility.Extensions;
using Transformalize.Extensions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {

    public class TransformalizeService : ITransformalizeService, ITransformalizeJobService {

        protected const string DefaultFormat = "xml";
        protected const string DefaultFlavor = "attributes";
        protected const string DefaultMode = "default";

        private readonly IOrchardServices _orchardServices;
        private readonly IFileService _fileService;
        private readonly IAppDataFolder _appDataFolder;
        private readonly IExtensionManager _extensionManager;
        private readonly IReportsManager _reportsManager;
        private readonly List<int> _filesCreated = new List<int>();
        private static readonly string OrchardVersion = typeof(ContentItem).Assembly.GetName().Version.ToString();

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public IEnumerable<int> FilesCreated { get { return _filesCreated; } }

        public TransformalizeService(
            IOrchardServices orchardServices,
            IFileService fileService,
            IAppDataFolder appDataFolder,
            IExtensionManager extensionManager,
            IReportsManager reportsManager
            ) {
            _orchardServices = orchardServices;
            _fileService = fileService;
            _appDataFolder = appDataFolder;
            _extensionManager = extensionManager;
            _reportsManager = reportsManager;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public void InitializeFiles(ConfigurationPart part, IDictionary<string, string> query) {
            _filesCreated.Clear();
            if (query.ContainsKey("InputFile") && part.RequiresInputFile() == true)
                InitializeFile(part, query, "InputFile");
            if (query.ContainsKey("OutputFile") && part.RequiresOutputFile() == true)
                InitializeFile(part, query, "OutputFile");
        }

        public IEnumerable<ConfigurationPart> GetConfigurations() {
            return _orchardServices.ContentManager.Query<ConfigurationPart, ConfigurationPartRecord>(VersionOptions.Latest)
                .Join<TitlePartRecord>()
                .OrderBy(br => br.Title)
                .List();
        }

        public IEnumerable<ConfigurationPart> GetAuthorizedConfigurations() {
            return GetConfigurations().Where(c => _orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, c));
        }

        public ConfigurationPart GetConfiguration(int id) {
            return _orchardServices.ContentManager.Get(id, VersionOptions.Published).As<ConfigurationPart>();
        }

        public TransformalizeResponse Run(TransformalizeRequest request) {

            var moduleVersion = _extensionManager.GetExtension("Transformalize.Orchard").Version;
            var logger = new TransformalizeLogger(request.Part.Title(), request.Part.LogLevel, Logger, OrchardVersion, moduleVersion);
            var processes = new List<Process>();

            //transitioning to using TflRoot instead of string configuration
            if (request.Root != null) {
                processes.AddRange(ProcessFactory.Create(request.Root, logger, request.Options));
            } else {  //legacy
                processes.AddRange(ProcessFactory.Create(request.Configuration, logger, request.Options, request.Query));
            }

            for (var i = 0; i < processes.Count; i++) {
                var process = processes[i];
                CreateInputOperation(process, request);
                process.ExecuteScaler();
            }

            return new TransformalizeResponse() {
                Processes = processes.ToArray(),
                Log = logger.Dump().ToList()
            };
        }

        public Dictionary<string, string> GetQuery() {

            var context = System.Web.HttpContext.Current;
            var collection = context == null ?
                new NameValueCollection() :
                new NameValueCollection { context.Request.Form, context.Request.QueryString };
            var result = new Dictionary<string, string>(collection.Count, StringComparer.OrdinalIgnoreCase);

            foreach (var key in collection.AllKeys) {
                result[key] = collection[key];
            }
            if (!result.ContainsKey("flavor")) {
                result["flavor"] = DefaultFlavor;
            }
            if (!result.ContainsKey("format")) {
                result["format"] = DefaultFormat;
            }
            if (!result.ContainsKey("mode")) {
                result["mode"] = DefaultMode;
            }
            return result;
        }

        public void Run(string args) {

            var split = args.Split(',');
            var id = Convert.ToInt32(split[0]);
            var mode = split[1];

            var part = GetConfiguration(id);
            var request = new TransformalizeRequest(part, GetQuery(), null, Logger) { Options = new Options() { Mode = mode } };

            if (request.Root.Errors().Any()) {
                foreach (var error in request.Root.Errors()) {
                    Logger.Error(error);
                }
            } else {
                var response = Run(request);

                if (!response.Log.Any())
                    return;

                var name = TransformalizeTaskHandler.GetName(part.Title());
                var reportId = _reportsManager.CreateReport(mode.Left(1).ToUpper() + mode.Substring(1), name);
                var report = _reportsManager.Get(reportId);

                var status = string.Empty;
                foreach (var log in response.Log) {
                    var level = log[1].Left(4);
                    var utc = Convert.ToDateTime(log[0]).ToUniversalTime();
                    var data = log.Skip(3);
                    switch (level) {
                        case "erro":
                            report.Entries.Add(new ReportEntry { Message = string.Join(" | ", data), Type = ReportEntryType.Error, Utc = utc });
                            status = "with Error(s)";
                            break;
                        case "warn":
                            report.Entries.Add(new ReportEntry { Message = string.Join(" | ", data), Type = ReportEntryType.Warning, Utc = utc });
                            if (status != "with Error(s)") {
                                status = "with Warning(s)";
                            }
                            break;
                        default:
                            report.Entries.Add(new ReportEntry { Message = string.Join(" | ", data), Type = ReportEntryType.Information, Utc = utc });
                            break;
                    }
                }
                report.ActivityName += " " + status;
                _reportsManager.Flush();
            }
        }

        private static void CreateInputOperation(Process process, TransformalizeRequest request) {

            if (!request.Query.ContainsKey("data")) {
                return;
            }

            if (!process.Connections.Contains("input"))
                return;
            if (process.Connections.GetConnectionByName("input").Connection.Type != ProviderType.Internal)
                return;

            var rows = new List<Row>();
            var sr = new StringReader(request.Query["data"].ToString());
            var reader = new JsonTextReader(sr);
            reader.Read();
            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonException("The data passed into this process must be an array of arrays that contain input data matching the configured fields.  E.g. [['Dale','Newman', 34],['Gavin','Newman', 3]]");

            var entity = process.Entities.First();
            var inputFields = entity.InputFields();
            var conversion = Common.GetObjectConversionMap();

            // Note: Input 
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartArray) {
                    var row = new Row();
                    foreach (var field in inputFields) {
                        reader.Read();
                        row[field.Name] = conversion[field.SimpleType](reader.Value);
                    }
                    rows.Add(row);
                } else if (reader.TokenType == JsonToken.StartObject) {
                    var row = new Row();
                    do {
                        reader.Read();
                        if (reader.TokenType == JsonToken.PropertyName) {
                            var name = reader.Value.ToString();
                            reader.Read();
                            row[name] = reader.Value;
                        }
                    } while (reader.TokenType != JsonToken.EndObject);

                    foreach (var field in inputFields) {
                        row[field.Name] = conversion[field.SimpleType](row[field.Name]);
                    }
                    rows.Add(row);
                }
            }

            entity.InputOperation = new RowsOperation(rows);
        }

        private void InitializeFile(ConfigurationPart part, IDictionary<string, string> query, string key) {

            FilePart filePart;
            int id;

            var file = query[key] ?? "0";

            if (int.TryParse(file, out id)) {
                if (id > 0) {
                    filePart = _fileService.Get(id) ?? CreateOutputFile(part);
                } else {
                    filePart = CreateOutputFile(part);
                }
            } else {
                if (file.IndexOfAny(Path.GetInvalidPathChars()) == -1 && _appDataFolder.FileExists(file)) {
                    filePart = _fileService.Create(file);
                } else {
                    Logger.Error("File '{0}' passed into process is invalid!", file);
                    return;
                }
            }

            query[key] = filePart.FullPath;
        }

        private FilePart CreateOutputFile(ConfigurationPart part) {
            var prefix = Slugify(part.As<TitlePart>().Title);
            var filePart = _fileService.Create(prefix, part.OutputFileExtension);
            _filesCreated.Add(filePart.Id);
            return filePart;
        }

        private static string Slugify(string input) {
            var disallowed = new Regex(@"[/:?#\[\]@!$&'()*+,.;=\s\""\<\>\\\|%]+");
            var cleanedSlug = disallowed.Replace(input, "-").Trim('-', '.');

            cleanedSlug = Regex.Replace(cleanedSlug, @"\-{2,}", "-");

            if (cleanedSlug.Length > 1000)
                cleanedSlug = cleanedSlug.Substring(0, 1000).Trim('-', '.');

            return cleanedSlug.ToLower().RemoveDiacritics();
        }

    }
}