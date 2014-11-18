using System.Collections.Generic;
using System.IO;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.FileSystems.AppData;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Services;
using Orchard.UI.Notify;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {

    public class FileService : IFileService {
        private readonly IOrchardServices _orchardServices;
        private readonly IAppDataFolder _appDataFolder;
        private readonly IClock _clock;

        const string TRANSFORMALIZE_FOLDER = "Transformalize";
        const string FILE_TIMESTAMP = "yyyy-MM-dd-HH-mm-ss";

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

        public FilePart Upload(HttpPostedFileBase input) {
            
            var part = _orchardServices.ContentManager.New<FilePart>("File");
            var exportFile = string.Format("{0}-{1}-{2}",
                _orchardServices.WorkContext.CurrentUser.UserName,
                _clock.UtcNow.ToString(FILE_TIMESTAMP),
                Path.GetFileName(input.FileName)
                );
            if (!_appDataFolder.DirectoryExists(TRANSFORMALIZE_FOLDER)) {
                _appDataFolder.CreateDirectory(TRANSFORMALIZE_FOLDER);
            }

            part.FullPath = _appDataFolder.MapPath(_appDataFolder.Combine(TRANSFORMALIZE_FOLDER, exportFile));
            part.Direction = "In";
            input.SaveAs(part.FullPath);
            _orchardServices.ContentManager.Create(part);
            _orchardServices.Notifier.Information(T("{0} uploaded successfully.", Path.GetFileName(input.FileName)));

            return part;
        }

        public FilePart Create(string name, string extension) {

            var file = string.Format("{0}-{1}-{2}.{3}",
                _orchardServices.WorkContext.CurrentUser.UserName,
                _clock.UtcNow.ToString(FILE_TIMESTAMP),
                name,
                extension.TrimStart(".".ToCharArray())
            );
            if (!_appDataFolder.DirectoryExists(TRANSFORMALIZE_FOLDER)) {
                _appDataFolder.CreateDirectory(TRANSFORMALIZE_FOLDER);
            }

            var path = _appDataFolder.Combine(TRANSFORMALIZE_FOLDER, file);
            _appDataFolder.CreateFile(path, string.Empty);

            var part = _orchardServices.ContentManager.New<FilePart>("File");
            part.FullPath = _appDataFolder.MapPath(path);
            part.Direction = "Out";
            _orchardServices.ContentManager.Create(part);

            return part;
        }

        public IEnumerable<FilePart> GetFiles() {
            var isSiteOwner = _orchardServices.Authorizer.Authorize(global::Orchard.Security.StandardPermissions.SiteOwner);
            if (isSiteOwner) {
                return _orchardServices.ContentManager.Query<FilePart, FilePartRecord>("File")
                    .Join<CommonPartRecord>()
                    .OrderByDescending(cpr => cpr.CreatedUtc)
                    .List();
            }
            return _orchardServices.ContentManager.Query<FilePart, FilePartRecord>("File")
                .Join<CommonPartRecord>()
                .Where(cpr => cpr.OwnerId == _orchardServices.WorkContext.CurrentUser.Id)
                .OrderByDescending(cpr => cpr.CreatedUtc)
                .List();
        }

        public FilePart Get(int id) {
            return _orchardServices.ContentManager.Get(id).As<FilePart>();
        }

    }
}