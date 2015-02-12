using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

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

        [HttpPost]
        public ActionResult Upload() {

            if (!User.Identity.IsAuthenticated) {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.Upload))
                return new HttpUnauthorizedResult();

            if (Request.Files != null && Request.Files.Count > 0) {
                var input = Request.Files.Get(0);
                if (input != null && input.ContentLength > 0) {
                    var filePart = _fileService.Upload(input);
                    return RedirectToAction("Files", new { id = filePart.Id });
                }
                _orchardServices.Notifier.Error(T("Please choose a file."));
            } else {
                _orchardServices.Notifier.Error(T("Please choose a file."));
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult Files(int id) {

            if (User.Identity.IsAuthenticated) {

                ViewBag.CurrentId = id;
                ViewBag.SelectFor = Convert.ToInt32(Request.QueryString["SelectFor"] ?? "0");
                ViewBag.Mode = Request.QueryString["Mode"] ?? string.Empty;

                var response = new FilesResponse() {
                    TimeZoneInfo = _orchardServices.WorkContext.CurrentTimeZone,
                    Files = _fileService.GetFiles()
                };

                return View(response);
            }

            System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            return null;
        }

        [HttpGet]
        public ActionResult Download(int id) {

            var result = CheckFile(id);
            if (result.ActionResult != null)
                return result.ActionResult;

            if (!result.FileInfo.Exists) {
                return new HttpNotFoundResult();
            }

            var ext = Path.GetExtension(result.FilePart.FullPath) ?? "csv";
            return new FilePathResult(
                result.FileInfo.FullName,
                "application/" + ext.TrimStart(new[] { '.' })) {
                    FileDownloadName = result.FileInfo.Name
                };
        }

        public ActionResult Delete(int id) {

            var result = CheckFile(id);
            if (result.ActionResult != null)
                return result.ActionResult;

            try {
                _orchardServices.ContentManager.Remove(result.FilePart.ContentItem);
                if(result.FileInfo.Exists)
                    result.FileInfo.Delete();
            } catch (Exception ex) {
                _orchardServices.Notifier.Add(NotifyType.Error, T(ex.Message));
            }

            return RedirectToAction("Files");
        }

        private CheckFileResult CheckFile(int id) {
            if (!User.Identity.IsAuthenticated)
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);

            var part = _fileService.Get(id);

            if (part == null)
                return new CheckFileResult(new HttpNotFoundResult());

            if (string.IsNullOrEmpty(part.FullPath))
                return new CheckFileResult(new HttpNotFoundResult(), part);

            if (!_orchardServices.Authorizer.Authorize(global::Orchard.Security.StandardPermissions.SiteOwner)) {
                var commonPart = part.As<CommonPart>();
                if (!commonPart.Owner.UserName.Equals(User.Identity.Name))
                    return new CheckFileResult(new HttpUnauthorizedResult(), part);
            }

            return new CheckFileResult(null, part, new FileInfo(part.FullPath));
        }

    }
}