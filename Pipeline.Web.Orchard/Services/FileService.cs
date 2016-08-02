using System.Collections.Generic;
using System.IO;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.AppData;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Services;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Services {

    [OrchardFeature("Pipeline.Files")]
    public class FileService : IFileService {
        private readonly IOrchardServices _orchardServices;
        private readonly IAppDataFolder _appDataFolder;
        private readonly IClock _clock;

        const string FileFolder = "Transformalize";
        const string FileTimestamp = "yyyy-MM-dd-HH-mm-ss";

        public FileService(
            IOrchardServices orchardServices,
            IAppDataFolder appDataFolder,
            IClock clock) {
            _orchardServices = orchardServices;
            _appDataFolder = appDataFolder;
            _clock = clock;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public PipelineFilePart Upload(HttpPostedFileBase input) {

            var part = _orchardServices.ContentManager.New<PipelineFilePart>(Common.PipelineFileName);

            part.Settings["ContentPermissionsPartSettings.View"] = "Anonymous";  // a test

            var exportFile = string.Format("{0}-{1}-{2}",
                _orchardServices.WorkContext.CurrentUser.UserName,
                _clock.UtcNow.ToString(FileTimestamp),
                Path.GetFileName(input.FileName)
            );

            if (!_appDataFolder.DirectoryExists(FileFolder)) {
                _appDataFolder.CreateDirectory(FileFolder);
            }

            part.FullPath = _appDataFolder.MapPath(_appDataFolder.Combine(FileFolder, exportFile));
            part.Direction = "In";
            input.SaveAs(part.FullPath);
            _orchardServices.ContentManager.Create(part);

            _orchardServices.Notifier.Information(T("{0} uploaded successfully.", Path.GetFileName(input.FileName)));

            return part;
        }

        public PipelineFilePart Create(string fileName) {
            var part = _orchardServices.ContentManager.New<PipelineFilePart>(Common.PipelineFileName);
            part.FullPath = fileName;
            part.Direction = "Out";
            _orchardServices.ContentManager.Create(part);
            return part;
        }

        public PipelineFilePart Create(string name, string extension) {

            var file = string.Format("{0}-{1}-{2}.{3}",
                _orchardServices.WorkContext.CurrentUser.UserName,
                _clock.UtcNow.ToString(FileTimestamp),
                name,
                extension.TrimStart(".".ToCharArray())
            );
            if (!_appDataFolder.DirectoryExists(FileFolder)) {
                _appDataFolder.CreateDirectory(FileFolder);
            }

            var path = _appDataFolder.Combine(FileFolder, file);
            _appDataFolder.CreateFile(path, string.Empty);

            var part = _orchardServices.ContentManager.New<PipelineFilePart>(Common.PipelineFileName);
            part.FullPath = _appDataFolder.MapPath(path);
            part.Direction = "Out";
            _orchardServices.ContentManager.Create(part);

            return part;
        }

        public IEnumerable<PipelineFilePart> List() {
            var isSiteOwner = _orchardServices.Authorizer.Authorize(global::Orchard.Security.StandardPermissions.SiteOwner);
            if (isSiteOwner) {
                return _orchardServices.ContentManager.Query<PipelineFilePart, PipelineFilePartRecord>(VersionOptions.Published)
                    .Join<CommonPartRecord>()
                    .OrderByDescending(cpr => cpr.CreatedUtc)
                    .List();
            }
            return _orchardServices.ContentManager.Query<PipelineFilePart, PipelineFilePartRecord>(VersionOptions.Published)
                .Join<CommonPartRecord>()
                .Where(cpr => cpr.OwnerId == _orchardServices.WorkContext.CurrentUser.Id)
                .OrderByDescending(cpr => cpr.CreatedUtc)
                .List();
        }

        public PipelineFilePart Get(int id) {
            return _orchardServices.ContentManager.Get(id).As<PipelineFilePart>();
        }

    }
}