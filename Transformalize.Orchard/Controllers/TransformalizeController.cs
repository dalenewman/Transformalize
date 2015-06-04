using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Features;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Logging;
using Transformalize.Extensions;
using Transformalize.Main.Providers;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    public class TransformalizeController : Controller {

        private readonly IOrchardServices _orchardServices;
        private readonly ITransformalizeService _transformalize;
        private readonly IFileService _fileService;
        private readonly IFeatureManager _featureManager;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public TransformalizeController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IFileService fileService,
            IFeatureManager featureManager
        ) {
            _orchardServices = services;
            _transformalize = transformalize;
            _fileService = fileService;
            _featureManager = featureManager;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        [Themed]
        public ActionResult Configurations(int id)
        {

            if (!User.Identity.IsAuthenticated)
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);

            var inputFileId = Convert.ToInt32(Request.QueryString["InputFile"] ?? "0");
            var outputFileId = Convert.ToInt32(Request.QueryString["OutputFile"] ?? "0");
            var mode = Request.QueryString["mode"] ?? string.Empty;

            var viewModel = new Configurations(_fileService) {
                ConfigurationParts = _transformalize.GetAuthorizedConfigurations(),
                InputFileId = inputFileId,
                OutputFileId = outputFileId,
                CurrentId = id,
                Mode = mode,
                Edit = _orchardServices.Authorizer.Authorize(global::Orchard.Security.StandardPermissions.SiteOwner),
                Enqueue = _featureManager.GetEnabledFeatures().Any(f => f.Id == "Orchard.JobsQueue")
            };

            return View(viewModel);
        }

        [Themed]
        public ActionResult Execute(int id) {

            if (id == 0)
                return new HttpNotFoundResult();

            if (!Request.IsLocal) {
                if (!User.Identity.IsAuthenticated)
                    System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
                if (!_orchardServices.Authorizer.Authorize(Permissions.Execute))
                    return new HttpUnauthorizedResult();
            }

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null)
                return new HttpNotFoundResult();

            if (!Request.IsLocal) {
                if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                    return new HttpUnauthorizedResult();
                }
            }

            var query = _transformalize.GetQuery();

            // files
            HandleInputFile(part, query);
            _transformalize.InitializeFiles(part, query);

            // ready
            var viewModel = Run(new TransformalizeRequest(part, query, null, Logger));

            var returnUrl = (Request.Form["ReturnUrl"] ?? Request.QueryString["ReturnUrl"]) ?? string.Empty;
            if (!returnUrl.Equals(string.Empty))
                return new RedirectResult(returnUrl);

            if (!part.DisplayLog && _transformalize.FilesCreated.Any()) {
                if (_transformalize.FilesCreated.Any()) {
                    var fileCount = _transformalize.FilesCreated.Count();
                    _orchardServices.Notifier.Information(T("You have {0} new file{0}.", fileCount, fileCount.Plural()));
                    return RedirectToAction("Download", "File", new { id = _transformalize.FilesCreated.Last() });
                }
                if (viewModel.TransformalizeResponse.Processes.All(p => p.OutputConnection.Type != ProviderType.Internal)) {
                    _orchardServices.Notifier.Add(NotifyType.Information, T(part.Title() + " Completed."));
                    return RedirectToAction("Configurations", "Transformalize", new { id = part.Id });
                }
            }

            ViewBag.CurrentId = id;
            return View(viewModel);
        }

        private void HandleInputFile(ConfigurationPart part, IDictionary<string, string> query) {
            if (Request.Files == null || Request.Files.Count <= 0 || part.RequiresInputFile() != true)
                return;

            var input = Request.Files.Get(0);

            if (input == null || input.ContentLength <= 0)
                return;

            var filePart = _fileService.Upload(input);
            query["InputFile"] = filePart.Id.ToString(CultureInfo.InvariantCulture);
        }

        private ExecuteViewModel Run(TransformalizeRequest request) {

            var model = new ExecuteViewModel { DisplayLog = request.Part.DisplayLog };

            if (request.Part.TryCatch) {
                try {
                    model.TransformalizeResponse = _transformalize.Run(request);
                } catch (Exception ex) {
                    model.DisplayLog = true;
                    model.TransformalizeResponse.Log.Add(new[]{DateTime.Now.ToString(), "error","orchard", ".", ex.Message});
                    model.TransformalizeResponse.Log.Add(new[] { DateTime.Now.ToString(), "debug", "orchard", ".", ex.StackTrace });
                    Logger.Error(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            } else {
                model.TransformalizeResponse = _transformalize.Run(request);
            }

            return model;
        }

    }
}