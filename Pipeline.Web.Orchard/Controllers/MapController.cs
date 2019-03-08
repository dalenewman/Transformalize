#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Orchard;
using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.FileSystems.AppData;
using Orchard.Logging;
using Orchard.Services;
using Orchard.Themes;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;
using Pipeline.Web.Orchard.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.SessionState;
using Transformalize.Contracts;
using LogLevel = Transformalize.Contracts.LogLevel;
using Permissions = Orchard.Core.Contents.Permissions;
using Process = Transformalize.Configuration.Process;

namespace Pipeline.Web.Orchard.Controllers {

    [ValidateInput(false), SessionState(SessionStateBehavior.ReadOnly), Themed]
    public class MapController : BaseController {

        private const string MissingMapboxToken = "Register a MapBox token in settings.";
        private const string MissingFile = "This transformalize expects a file.";

        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        private readonly ISecureFileService _secureFileService;
        private readonly IBatchCreateService _batchCreateService;
        private readonly IBatchWriteService _batchWriteService;
        private readonly IBatchRunService _batchRunService;
        private readonly IBatchRedirectService _batchRedirectService;

        public MapController(
            IOrchardServices services,
            IProcessService processService,
            ISecureFileService secureFileService,
            ICfgService cfgService,
            ISlugService slugService,
            IAppDataFolder appDataFolder,
            IClock clock,
            IBatchCreateService batchCreateService,
            IBatchWriteService batchWriteService,
            IBatchRunService batchRunService,
            IBatchRedirectService batchRedirectService,
            IFileService fileService
        ) {
            _orchardServices = services;
            _processService = processService;
            _secureFileService = secureFileService;
            _batchCreateService = batchCreateService;
            _batchWriteService = batchWriteService;
            _batchRunService = batchRunService;
            _batchRedirectService = batchRedirectService;
        }

        [Themed(true)]
        public ActionResult Index(int id) {

            var timer = new Stopwatch();
            timer.Start();

            var settings = _orchardServices.WorkContext.CurrentSite.As<PipelineSettingsPart>();

            if (string.IsNullOrEmpty(settings.MapBoxToken)) {
                _orchardServices.Notifier.Add(NotifyType.Warning, T(MissingMapboxToken));
                return new HttpStatusCodeResult(500, MissingMapboxToken);
            }

            var process = new Process { Name = "Map" };

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            if (part == null) {
                process.Name = "Not Found";
                return new HttpNotFoundResult();
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                return new HttpUnauthorizedResult();
            }

            process = _processService.Resolve(part);

            var parameters = Common.GetParameters(Request, _orchardServices, _secureFileService);

            if (part.NeedsInputFile && Convert.ToInt32(parameters[Common.InputFileIdName]) == 0) {
                _orchardServices.Notifier.Add(NotifyType.Error, T(MissingFile));
                return new HttpNotFoundResult(MissingFile);
            }

            GetStickyParameters(part.Id, parameters);

            process.Load(part.Configuration, parameters);

            if (process.Errors().Any()) {
                foreach (var error in process.Errors()) {
                    _orchardServices.Notifier.Add(NotifyType.Error, T(error));
                }
                return new HttpStatusCodeResult(500, "There are errors in the configuration.");
            }

            process.Mode = "map";
            process.Buffer = false; // no buffering for maps
            process.ReadOnly = true;  // force maps to omit system fields
            process.Pipeline = "parallel.linq";

            var reportParameters = process.GetActiveParameters();

            SetStickyParameters(part.Id, reportParameters);

            // secure actions
            var actions = process.Actions.Where(a => !a.Before && !a.After && !a.Description.StartsWith("Batch", StringComparison.OrdinalIgnoreCase));
            foreach (var action in actions) {
                var p = _orchardServices.ContentManager.Get(action.Id);
                if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, p)) {
                    action.Description = "BatchUnauthorized";
                }
            }

            var sizes = new List<int>();
            sizes.AddRange(part.Sizes(part.PageSizes));
            sizes.AddRange(part.Sizes(part.MapSizes));

            var stickySize = GetStickyUrlParameter(part.Id, "size", () => sizes.Min());

            Common.SetPageSize(process, parameters, sizes.Min(), stickySize, sizes.Max());

            if (Request.HttpMethod.Equals("POST") && parameters.ContainsKey("action")) {

                var system = reportParameters.FirstOrDefault(p => p.Name.Equals("System", StringComparison.OrdinalIgnoreCase));
                if (system == null) {
                    Logger.Error("The {0} requires a System parameter in order to run bulk actions.", part.Title());
                    _orchardServices.Notifier.Error(T("The {0} requires a System parameter in order to run bulk actions.", part.Title()));
                } else {
                    if (system.Value == "*") {
                        _orchardServices.Notifier.Warning(T("The {0} must be selected in order to run an action.", system.Label));
                        system = null;
                    }
                }

                var action = process.Actions.FirstOrDefault(a => a.Description == parameters["action"]);

                if (action != null && system != null) {

                    // check security
                    var actionPart = _orchardServices.ContentManager.Get(action.Id);
                    if (actionPart == null) {
                        return new HttpNotFoundResult(string.Format("The action id {0} does not refer to a content item id.", action.Id));
                    }

                    if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, actionPart)) {

                        // security okay
                        parameters["entity"] = process.Entities.First().Alias;
                        foreach (var p in reportParameters.Where(pr => pr.Name.ToLower() != "entity" && pr.Value != "*")) {
                            parameters[p.Name] = p.Value;
                        }

                        var batchParameters = _batchCreateService.Create(process, parameters);

                        Common.AddOrchardVariables(batchParameters, _orchardServices, Request);

                        batchParameters["count"] = parameters.ContainsKey("count") ? parameters["count"] : "0";
                        var count = _batchWriteService.Write(Request, process, batchParameters);

                        if (count > 0) {

                            if (!batchParameters.ContainsKey("system")) {
                                batchParameters["system"] = system.Value;
                            }

                            if (_batchRunService.Run(action, batchParameters)) {
                                if (action.Url == string.Empty) {
                                    if (batchParameters.ContainsKey("BatchId")) {
                                        _orchardServices.Notifier.Information(T(string.Format("Processed {0} records in batch {1}.", count, batchParameters["BatchId"])));
                                    } else {
                                        _orchardServices.Notifier.Information(T(string.Format("Processed {0} records.", count)));
                                    }
                                    var referrer = HttpContext.Request.UrlReferrer == null ? Url.Action("Index", new { Id = id }) : HttpContext.Request.UrlReferrer.ToString();
                                    return _batchRedirectService.Redirect(referrer, batchParameters);
                                }
                                return _batchRedirectService.Redirect(action.Url, batchParameters);
                            }

                            var message = batchParameters.ContainsKey("BatchId") ? string.Format("Batch {0} failed.", batchParameters["BatchId"]) : "Batch failed.";
                            Logger.Error(message);
                            _orchardServices.Notifier.Error(T(message));
                            foreach (var key in batchParameters.Keys) {
                                Logger.Error("Batch Parameter {0} = {1}.", key, batchParameters[key]);
                            }
                            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, message);
                        }
                    } else {
                        return new HttpUnauthorizedResult("You do not have access to this bulk action.");
                    }
                }

            }

            if (IsMissingRequiredParameters(reportParameters, _orchardServices.Notifier)) {
                return View(new ReportViewModel(process, part));
            }

            try {

                var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                runner.Execute(process);
                process.Request = "Execute";
                process.Time = timer.ElapsedMilliseconds;

                if (process.Log.Any(l => l.LogLevel == LogLevel.Error)) {
                    foreach (var error in process.Log.Where(l => l.LogLevel == LogLevel.Error)) {
                        _orchardServices.Notifier.Add(NotifyType.Error, T(error.Message));
                    }
                    process.Status = 500;
                    return new HttpStatusCodeResult(500);
                }

            } catch (Exception ex) {
                Logger.Error(ex, ex.Message);
                _orchardServices.Notifier.Error(T(ex.Message));
            }

            return View(new ReportViewModel(process, part));

        }

    }
}