using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.XmlRpc.Controllers;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;
using Transformalize.Extensions;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    public class TransformalizeController : Controller {

        private readonly IOrchardServices _orchardServices;
        private readonly ITransformalizeService _transformalize;
        private readonly IFileService _fileService;

        public Localizer T { get; set; }

        public TransformalizeController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IFileService fileService
        ) {
            _orchardServices = services;
            _transformalize = transformalize;
            _fileService = fileService;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Configurations(int id) {

            if (!User.Identity.IsAuthenticated)
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);

            var inputFileId = Convert.ToInt32(Request.QueryString["InputFile"] ?? "0");
            var outputFileId = Convert.ToInt32(Request.QueryString["OutputFile"] ?? "0");
            var mode = Request.QueryString["Mode"] ?? string.Empty;

            var viewModel = new Configurations(_fileService) {
                ConfigurationParts = _transformalize.GetAuthorizedConfigurations(),
                InputFileId = inputFileId,
                OutputFileId = outputFileId,
                CurrentId = id,
                Mode = mode,
                Edit = _orchardServices.Authorizer.Authorize(global::Orchard.Security.StandardPermissions.SiteOwner)
            };

            return View(viewModel);
        }

        [LiveWriterController.NoCache]
        public ActionResult Configuration(int id) {

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return new HttpNotFoundResult();
            }

            if (!(Request.IsLocal || part.IsInAllowedRange(Request.UserHostAddress))) {
                if (User.Identity.IsAuthenticated) {
                    if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                        return new HttpUnauthorizedResult();
                    }
                } else {
                    System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
                }
            }

            var query = new NameValueCollection(Request.QueryString);
            return new ContentResult() {
                Content = Encoding.UTF8.GetString(Encoding.Default.GetBytes(_transformalize.InjectParameters(part, query))),
                ContentEncoding = Encoding.UTF8,
                ContentType = "text/xml"
            };
        }

        public ActionResult MetaData(int id) {

            if (!User.Identity.IsAuthenticated) {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            }

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return new HttpNotFoundResult();
            }

            var query = new NameValueCollection(Request.QueryString);
            return new ContentResult() {
                Content = _transformalize.GetMetaData(part, query),
                ContentEncoding = Encoding.UTF8,
                ContentType = "text/xml"
            };
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

            // ready
            var query = GetQuery();

            // handle input files
            if (part.RequiresInputFile() == true) {
                if (Request.Files != null && Request.Files.Count > 0) {
                    var input = Request.Files.Get(0);
                    if (input != null && input.ContentLength > 0) {
                        var filePart = _fileService.Upload(input);
                        query.Remove("InputFile");
                        query.Add("InputFile", filePart.Id.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            var viewModel = Run(part, query);

            var returnUrl = (Request.Form["ReturnUrl"] ?? Request.QueryString["ReturnUrl"]) ?? string.Empty;
            if (!returnUrl.Equals(string.Empty))
                return new RedirectResult(returnUrl);

            if (!part.DisplayLog && _transformalize.FilesCreated.Any()) {
                if (_transformalize.FilesCreated.Any()) {
                    var fileCount = _transformalize.FilesCreated.Count();
                    _orchardServices.Notifier.Information(T("You have {0} new file{0}.", fileCount, fileCount.Plural()));
                    return RedirectToAction("Download", "File", new { id = _transformalize.FilesCreated.Last() });
                }
                if (viewModel.Processes.All(p => p.OutputConnection.Type != ProviderType.Internal)) {
                    return RedirectToAction("Configurations", "Transformalize", new { id = part.Id });
                }
            }

            ViewBag.CurrentId = id;
            return View(viewModel);
        }

        private NameValueCollection GetQuery() {
            var result = new NameValueCollection();
            if (Request.Form != null) {
                result.Add(Request.Form);
            }
            if (Request.QueryString != null) {
                result.Add(Request.QueryString);
            }
            return result;
        }

        private ExecuteViewModel Run(ConfigurationPart part, NameValueCollection query) {

            var model = new ExecuteViewModel() { DisplayLog = part.DisplayLog };
            var options = query["Mode"] != null ? new Options { Mode = query["Mode"] } : new Options();

            var log = new SynchronizedCollection<string>();
            var memory = new ObservableEventListener();

            if (part.DisplayLog) {
                memory.EnableEvents(TflEventSource.Log, part.ToLogLevel());
                memory.LogToMemory(ref log);
                TflLogger.Info("Orchard", "Log", "Injecting memory logger");
            }

            if (part.TryCatch) {
                try {
                    model.Processes = RunCommon(part, query, options);
                } catch (Exception ex) {
                    TflLogger.Error(string.Empty, string.Empty, ex.Message);
                    TflLogger.Warn(string.Empty, string.Empty, ex.StackTrace);
                    if (!part.DisplayLog) {
                        _orchardServices.Notifier.Add(NotifyType.Error, T(ex.Message));
                    }
                }
            } else {
                model.Processes = RunCommon(part, query, options);
            }

            if (!part.DisplayLog) {
                return model;
            }

            model.Log = log.ToArray();

            return model;
        }

        private Process[] RunCommon(ConfigurationPart part, NameValueCollection query, Options options) {
            var xml = _transformalize.InjectParameters(part, query);
            var processes = new List<Process>();
            if (options.Mode.Equals("rebuild", StringComparison.OrdinalIgnoreCase)) {
                options.Mode = "init";
                processes.AddRange(ProcessFactory.Create(xml, options));
                options.Mode = "first";
                processes.AddRange(ProcessFactory.Create(xml, options));
            } else {
                processes.AddRange(ProcessFactory.Create(xml, options));
            }

            foreach (var process in processes) {
                process.ExecuteScaler();
                if (!part.DisplayLog && !process.OutputConnection.Type.HasFlag(ProviderType.Internal)) {
                    _orchardServices.Notifier.Information(T("{0} executed successfully.", process.Name));
                }
            }

            return processes.ToArray();
        }

    }
}