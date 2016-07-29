using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Services;
using Orchard.Core.Contents;

namespace Pipeline.Web.Orchard.Controllers {

    [Themed]
    public class FileController : Controller {

        private readonly IFileService _fileService;
        private readonly IOrchardServices _orchardServices;

        public Localizer T { get; set; }

        public FileController(IOrchardServices services, IFileService fileService) {
            _orchardServices = services;
            _fileService = fileService;
            T = NullLocalizer.Instance;
        }

        [Themed(false)]
        [HttpPost]
        public ActionResult Upload() {

            if (!User.Identity.IsAuthenticated) {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            }

            if (Request.Files != null && Request.Files.Count > 0) {
                var input = Request.Files.Get(0);
                if (input != null && input.ContentLength > 0) {
                    var filePart = _fileService.Upload(input);
                    return RedirectToAction("List", new { id = filePart.Id });
                }
                _orchardServices.Notifier.Error(T("Please choose a file."));
            } else {
                _orchardServices.Notifier.Error(T("Please choose a file."));
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult List() {

            if (User.Identity.IsAuthenticated) {
                return View(_fileService.List());
            }

            System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            return null;
        }

        [ActionName("File/Download")]
        [Themed(false)]
        [HttpGet]
        public ActionResult Download(int id) {

            if (!User.Identity.IsAuthenticated)
                return new HttpUnauthorizedResult("You must be logged in to download files.");

            var part = _fileService.Get(id);

            if (part == null)
                return new HttpNotFoundResult("The file does not exist.");

            if (string.IsNullOrEmpty(part.FullPath))
                return new HttpNotFoundResult("The file path is empty.");

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                return new HttpUnauthorizedResult("You do not have permissions to view this file.");
            }

            var fileInfo = new FileInfo(part.FullPath);

            if (!fileInfo.Exists) {
                return new HttpNotFoundResult("The file does not exist anymore.");
            }

            var ext = Path.GetExtension(part.FullPath);
            return new FilePathResult(fileInfo.FullName, "application/" + ext.TrimStart('.')) { FileDownloadName = fileInfo.Name };
        }

        [ActionName("File/View")]
        [Themed(false)]
        public ActionResult View(int id) {
            if (!User.Identity.IsAuthenticated) {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            }

            var part = _fileService.Get(id);

            if (part == null || string.IsNullOrEmpty(part.FullPath)) {
                _orchardServices.Notifier.Add(NotifyType.Warning, T("The file you tried to view is missing."));
                return RedirectToAction("List");
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                _orchardServices.Notifier.Add(NotifyType.Warning, T("Sorry.  You do not have permission to view this file."));
                return RedirectToAction("List");
            }

            var fileInfo = new FileInfo(part.FullPath);

            if (!fileInfo.Exists) {
                _orchardServices.Notifier.Add(NotifyType.Warning, T("The file you tried to view is not available anymore."));
                return RedirectToAction("List");
            }

            return new ContentResult { Content = System.IO.File.ReadAllText(fileInfo.FullName), ContentType = "text/plain"};
        }

        [ActionName("File/Delete")]
        public ActionResult Delete(int id) {

            if (!User.Identity.IsAuthenticated) {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            }

            var part = _fileService.Get(id);

            if (part == null) {
                _orchardServices.Notifier.Add(NotifyType.Warning, T("The file you tried to delete is already gone."));
                return RedirectToAction("List");
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.DeleteContent, part)) {
                _orchardServices.Notifier.Add(NotifyType.Warning, T("You are not authorized to delete this file."));
                return RedirectToAction("List");
            }

            try {
                _orchardServices.ContentManager.Remove(part.ContentItem);
                if (string.IsNullOrEmpty(part.FullPath)) {
                    _orchardServices.Notifier.Add(NotifyType.Warning, T("The file path associated with this content item is empty."));
                } else {
                    var fileInfo = new FileInfo(part.FullPath);
                    if (fileInfo.Exists) {
                        fileInfo.Delete();
                        _orchardServices.Notifier.Add(NotifyType.Information, T("The file {0} is no more.", part.FileName()));
                    } else {
                        _orchardServices.Notifier.Add(NotifyType.Warning, T("The file associated with this content item no longer exists."));
                    }
                }
            } catch (Exception ex) {
                _orchardServices.Notifier.Add(NotifyType.Error, T("Does not compute!  My systems are malfunctioning. {0}", ex.Message));
            }

            return RedirectToAction("List");
        }


    }
}