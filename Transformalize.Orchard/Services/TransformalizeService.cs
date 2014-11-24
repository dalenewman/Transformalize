using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;
using Transformalize.Orchard.Models;
using Transformalize.Runner;

namespace Transformalize.Orchard.Services {

    public class TransformalizeService : ITransformalizeService {

        private readonly IOrchardServices _orchardServices;
        private readonly IFileService _fileService;
        private readonly List<int> _filesCreated = new List<int>();

        public TransformalizeService(
            IOrchardServices orchardServices,
            IFileService fileService
            ) {
            _orchardServices = orchardServices;
            _fileService = fileService;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public IEnumerable<int> FilesCreated { get { return _filesCreated; } }

        public void InjectParameters(ref ConfigurationPart part, NameValueCollection query) {
            if (part.GetParametersInjected())
                return;

            _filesCreated.Clear();
            if (query["__RequestVerificationToken"] != null) {
                query.Remove("__RequestVerificationToken");
            }
            InitializeFile(part, query, "InputFile");
            InitializeFile(part, query, "OutputFile");
            part.Configuration = new ContentsStringReader(query).Read(part.Configuration).Content;
            part.SetParametersInjected();
        }

        public string GetMetaData(ConfigurationPart part, NameValueCollection query) {
            InjectParameters(ref part, query);
            var process = ProcessFactory.Create(part.Configuration, new Options { Mode = "metadata" }).First();
            return new MetaDataWriter(process).Write();
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

        public TransformalizeResponse Run(ConfigurationPart part, Options options, NameValueCollection query) {

            var log = new List<string>();

            if (part.DisplayLog) {
                var memory = new ObservableEventListener();
                memory.EnableEvents(TflEventSource.Log, part.ToLogLevel());
                memory.LogToMemory(ref log);
                TflLogger.Info("Orchard", "Log", "Injecting memory logger");
            }

            var processes = new List<Process>();
            if (options.Mode.Equals("rebuild", StringComparison.OrdinalIgnoreCase)) {
                options.Mode = "init";
                processes.AddRange(ProcessFactory.Create(part.Configuration, options));
                options.Mode = "first";
                processes.AddRange(ProcessFactory.Create(part.Configuration, options));
            } else {
                processes.AddRange(ProcessFactory.Create(part.Configuration, options));
            }

            for (var i = 0; i < processes.Count; i++) {
                var process = processes[i];
                CreateInputOperation(ref process, query);
                process.ExecuteScaler();
            }

            return new TransformalizeResponse() {
                Processes = processes.ToArray(),
                Log = log
            };
        }

        private static void CreateInputOperation(ref Process process, NameValueCollection query) {

            if (!process.Connections.ContainsKey("input"))
                return;
            if (process.Connections["input"].Type != ProviderType.Internal)
                return;

            var data = query["data"];
            if (data == null)
                return;

            var rows = new List<Row>();
            var sr = new StringReader(data);
            var reader = new JsonTextReader(sr);
            reader.Read();
            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonException("The data passed into this process must be an array of arrays that contain input data matching the configured fields.  E.g. [['Dale','Newman', 34],['Gavin','Newman', 3]]");

            var entity = process.Entities.First();
            var inputFields = entity.InputFields();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartArray) {
                    var row = new Row();
                    foreach (var name in inputFields.Select(f => f.Name)) {
                        reader.Read();
                        row[name] = reader.Value;
                    }
                    rows.Add(row);
                } else if (reader.TokenType == JsonToken.StartObject) {
                    var row = new Row();
                    do {
                        reader.Read();
                        var name = reader.Value;
                        reader.Read();
                        var value = reader.Value;
                        row[name] = value;
                    } while (reader.TokenType != JsonToken.EndObject);
                    rows.Add(row);
                }
            }

            entity.InputOperation = new RowsOperation(rows);
        }

        private void InitializeFile(ConfigurationPart part, NameValueCollection query, string key) {

            if (!part.Configuration.Contains("@(" + key + ")") || !query.AllKeys.Any(k => k.Equals(key))) {
                return;
            }

            int id;
            if (!int.TryParse(query[key] ?? "0", out id)) {
                _orchardServices.Notifier.Add(NotifyType.Error, T("{0} must be an integer. \"{1}\" is not valid.", key, query[key]));
                return;
            }

            FilePart filePart;
            if (id > 0) {
                filePart = _fileService.Get(id) ?? CreateOutputFile(part);
            } else {
                filePart = CreateOutputFile(part);
            }

            if (query[key] != null)
                query.Remove(key);

            query.Add(key, filePart.FullPath);
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