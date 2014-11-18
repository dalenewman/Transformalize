using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using Transformalize.Main;
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

        public string InjectParameters(ConfigurationPart part, NameValueCollection query) {
            _filesCreated.Clear();
            if (query["__RequestVerificationToken"] != null) {
                query.Remove("__RequestVerificationToken");
            }
            InitializeFile(part, query, "InputFile");
            InitializeFile(part, query, "OutputFile");
            return new ContentsStringReader(query).Read(part.Configuration).Content;
        }

        public string GetMetaData(ConfigurationPart part, NameValueCollection query) {
            var process = ProcessFactory.Create(InjectParameters(part, query), new Options { Mode = "metadata"}).First();
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