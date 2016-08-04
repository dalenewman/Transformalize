using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Services;
using Orchard.Core.Contents;
using Orchard.Environment.Extensions;

namespace Pipeline.Web.Orchard.Controllers {

    [OrchardFeature("Pipeline.Files")]
    [ValidateInput(false), Themed]
    public class FileController : Controller {

        private readonly IFileService _fileService;
        private readonly IOrchardServices _orchardServices;
        private readonly ISecureFileService _secureFileService;

        public Localizer T { get; set; }

        public FileController(
            IOrchardServices services,
            IFileService fileService,
            ISecureFileService secureFileService
        ) {
            _orchardServices = services;
            _fileService = fileService;
            _secureFileService = secureFileService;
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
                    var filePart = _fileService.Upload(input, Request.Form["Role"]);
                    return RedirectToAction("List", new { id = filePart.Id });
                }
                _orchardServices.Notifier.Error(T("Please choose a file."));
            } else {
                _orchardServices.Notifier.Error(T("Please choose a file."));
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult List() {
            return View(_fileService.List().Where(f => _orchardServices.Authorizer.Authorize(Permissions.ViewContent, f)));
        }

        [ActionName("File/Download")]
        [Themed(false)]
        [HttpGet]
        public ActionResult Download(int id) {

            var response = _secureFileService.Get(id);
            if (response.Status == 200) {
                return new FilePathResult(new FileInfo(response.Part.FullPath).FullName, response.Part.MimeType()) {
                    FileDownloadName = response.Part.FileName()
                };
            }

            return response.ToActionResult();
        }

        [ActionName("File/View")]
        [Themed(false)]
        public ActionResult View(int id) {

            var response = _secureFileService.Get(id);

            if (response.Status == 200) {
                var mimeType = response.Part.MimeType();
                var fileInfo = new FileInfo(response.Part.FullPath);
                if (mimeType.StartsWith("text")) {
                    return new ContentResult {
                        Content = System.IO.File.ReadAllText(fileInfo.FullName),
                        ContentType = mimeType
                    };
                }
                return new FileContentResult(System.IO.File.ReadAllBytes(fileInfo.FullName), mimeType);
            }

            _orchardServices.Notifier.Add(NotifyType.Warning, T(response.Message));
            return RedirectToAction("List");
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
                _fileService.Delete(part);
            } catch (Exception ex) {
                _orchardServices.Notifier.Add(NotifyType.Error, T("Does not compute!  My systems are malfunctioning. {0}", ex.Message));
            }

            return RedirectToAction("List");
        }


    }
}