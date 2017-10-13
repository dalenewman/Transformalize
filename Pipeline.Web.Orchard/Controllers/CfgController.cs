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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using Transformalize.Contracts;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;
using Orchard.Autoroute.Services;
using Orchard.FileSystems.AppData;
using Orchard.Services;
using Pipeline.Web.Orchard.Services.Contracts;
using Process = Transformalize.Configuration.Process;
using Transformalize.Extensions;

namespace Pipeline.Web.Orchard.Controllers {

    [ValidateInput(false), Themed]
    public class CfgController : Controller {

        private static readonly HashSet<string> _renderedOutputs = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "map", "page" };

        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        private readonly ISortService _sortService;
        private readonly ISecureFileService _secureFileService;
        private readonly ICfgService _cfgService;
        private readonly ISlugService _slugService;
        private readonly IAppDataFolder _appDataFolder;
        private readonly IBatchCreateService _batchCreateService;
        private readonly IBatchWriteService _batchWriteService;
        private readonly IBatchRunService _batchRunService;
        private readonly IBatchRedirectService _batchRedirectService;
        private readonly IFileService _fileService;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public CfgController(
            IOrchardServices services,
            IProcessService processService,
            ISortService sortService,
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
            _appDataFolder = appDataFolder;
            _orchardServices = services;
            _processService = processService;
            _secureFileService = secureFileService;
            _cfgService = cfgService;
            _sortService = sortService;
            _slugService = slugService;
            _batchCreateService = batchCreateService;
            _batchWriteService = batchWriteService;
            _batchRunService = batchRunService;
            _batchRedirectService = batchRedirectService;
            _fileService = fileService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        [Themed(true)]
        public ActionResult List(string tagFilter) {

            // Sticky Tag Filter
            if (Request.RawUrl.EndsWith("List") || Request.RawUrl.Contains("List?")) {
                tagFilter = Session[Common.TagFilterName] != null ? Session[Common.TagFilterName].ToString() : Common.AllTag;
            } else {
                Session[Common.TagFilterName] = tagFilter;
            }

            if (!User.Identity.IsAuthenticated)
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);

            var viewModel = new ConfigurationListViewModel(
                _cfgService.List(tagFilter),
                Common.Tags<PipelineConfigurationPart, PipelineConfigurationPartRecord>(_orchardServices),
                tagFilter
            );

            return View(viewModel);
        }



        [Themed(true)]
        public ActionResult Form(int id) {
            return FormLogic(id);
        }

        [Themed(false)]
        public ActionResult Formless(int id) {
            return FormLogic(id);
        }

        private ActionResult FormLogic(int id) {
            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                return new HttpNotFoundResult("Form not found.");
            }
            var user = _orchardServices.WorkContext.CurrentUser == null ? "Anonymous" : _orchardServices.WorkContext.CurrentUser.UserName ?? "Anonymous";

            if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                var process = _processService.Resolve(part);
                var parameters = Common.GetParameters(Request, _secureFileService, _orchardServices);
                process.Load(part.Configuration, parameters);

                if (process.Errors().Any() || process.Warnings().Any()) {
                    return View(new FormViewModel(part, process));
                }

                if (Request.HttpMethod.Equals("POST")) {
                    var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                    var entity = process.Entities.First();

                    try {
                        runner.Execute(process);

                        if (entity.Rows.Count == 1 && (bool)entity.Rows[0][entity.ValidField]) {
                            // reset, modify for actual insert, and execute again
                            process = _processService.Resolve(part);
                            process.Load(part.Configuration, parameters);
                            var insert = entity.GetPrimaryKey().All(k => parameters.ContainsKey(k.Alias) && k.Default == parameters[k.Alias]);
                            process.Actions.Add(new Transformalize.Configuration.Action { After = true, Before = false, Type = "run", Connection = entity.Connection, Command = insert ? entity.InsertCommand : entity.UpdateCommand, Key = Guid.NewGuid().ToString() });

                            foreach (var field in entity.Fields.Where(f => f.Input && f.InputType == "file")) {
                                if (Request.Files != null && Request.Files.Count > 0) {
                                    var input = Request.Files.Get(field.Alias);
                                    if (input != null && input.ContentLength > 0) {
                                        var filePart = _fileService.Upload(input, "Authenticated", "Forms");
                                        var parameter = process.GetActiveParameters().FirstOrDefault(p => p.Name == field.Alias);
                                        if (parameter != null) {
                                            parameter.Value = Url.Action("View", "File", new { id = filePart.Id }) ?? string.Empty;
                                        }
                                    }
                                }
                            }

                            try {
                                runner.Execute(process);
                                _orchardServices.Notifier.Information(insert ? T("{0} inserted", process.Name) : T("{0} updated", process.Name));
                                return Redirect(parameters["Orchard.ReturnUrl"]);
                            } catch (Exception ex) {
                                _orchardServices.Notifier.Error(T("The {0} save failed: {2}", process.Name, ex.Message));
                            }
                        }
                    } catch (Exception ex) {
                        _orchardServices.Notifier.Error(T("The {0} save failed: {2}", process.Name, ex.Message));
                        return View(new FormViewModel(part, process));
                    }
                }
                return View(new FormViewModel(part, process));
            }

            _orchardServices.Notifier.Warning(user == "Anonymous" ? T("Anonymous users do not have permission to view this form. You may need to login.") : T("Sorry {0}. You do not have permission to view this form.", user));
            return new HttpUnauthorizedResult();
        }

        [Themed(false)]
        public ActionResult FormContent(int id) {

            var process = new Process { Name = "Form", Id = id };
            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                process.Status = 404;
                process.Message = "Not Found";
            } else {
                if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                    var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                    process = _processService.Resolve(part);
                    var parameters = Common.GetParameters(Request, _secureFileService, _orchardServices);
                    process.Load(part.Configuration, parameters);
                    runner.Execute(process);

                } else {
                    process.Message = "Unauthorized";
                    process.Status = 401;
                }
            }
            return View(process);
        }

        [Themed(true)]
        public ActionResult Report(int id) {

            var timer = new Stopwatch();
            timer.Start();

            var process = new Process { Name = "Report" };

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                process.Name = "Not Found";
            } else {

                var user = _orchardServices.WorkContext.CurrentUser == null ? "Anonymous" : _orchardServices.WorkContext.CurrentUser.UserName ?? "Anonymous";

                if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                    process = _processService.Resolve(part);

                    var parameters = Common.GetParameters(Request, _secureFileService, _orchardServices);
                    if (part.NeedsInputFile && Convert.ToInt32(parameters[Common.InputFileIdName]) == 0) {
                        _orchardServices.Notifier.Add(NotifyType.Error, T("This transformalize expects a file."));
                        process.Name = "File Not Found";
                    }

                    process.Load(part.Configuration, parameters);
                    process.Buffer = false; // no buffering for reports
                    process.ReadOnly = true;  // force reporting to omit system fields

                    // secure actions
                    var actions = process.Actions.Where(a => !a.Before && !a.After && !a.Description.StartsWith("Batch", StringComparison.OrdinalIgnoreCase));
                    foreach (var action in actions) {
                        var p = _orchardServices.ContentManager.Get(action.Id);
                        if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, p)) {
                            action.Description = "BatchUnauthorized";
                        }
                    }

                    var output = process.Output();

                    if (output.Provider.In("internal", "file")) {

                        Common.TranslatePageParametersToEntities(process, parameters, "page");

                        // change process for export and batch purposes
                        var reportType = Request["output"] ?? "page";
                        if (!_renderedOutputs.Contains(reportType)) {

                            if (reportType == "batch" && Request.HttpMethod.Equals("POST") && parameters.ContainsKey("action")) {

                                var action = process.Actions.FirstOrDefault(a => a.Description == parameters["action"]);

                                if (action != null) {

                                    // check security
                                    var actionPart = _orchardServices.ContentManager.Get(action.Id);
                                    if (actionPart != null && _orchardServices.Authorizer.Authorize(Permissions.ViewContent, actionPart)) {

                                        // security okay
                                        parameters["entity"] = process.Entities.First().Alias;
                                        var batchParameters = _batchCreateService.Create(process, parameters);

                                        Common.AddOrchardVariables(batchParameters, _orchardServices, Request);

                                        batchParameters["count"] = parameters.ContainsKey("count") ? parameters["count"] : "0";
                                        var count = _batchWriteService.Write(Request, process, batchParameters);

                                        if (count > 0) {

                                            if (_batchRunService.Run(action, batchParameters)) {
                                                if (action.Url == string.Empty) {
                                                    if (batchParameters.ContainsKey("BatchId")) {
                                                        _orchardServices.Notifier.Information(T(string.Format("Processed {0} records in batch {1}.", count, batchParameters["BatchId"])));
                                                    } else {
                                                        _orchardServices.Notifier.Information(T(string.Format("Processed {0} records.", count)));
                                                    }
                                                    var referrer = HttpContext.Request.UrlReferrer == null ? Url.Action("Report", new { Id = id}) : HttpContext.Request.UrlReferrer.ToString();
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

                            } else { // export
                                ConvertToExport(user, process, part, reportType, parameters);
                                process.Load(process.Serialize(), parameters);
                            }
                        }

                        if (Request["sort"] != null) {
                            _sortService.AddSortToEntity(process.Entities.First(), Request["sort"]);
                        }

                        if (process.Errors().Any()) {
                            foreach (var error in process.Errors()) {
                                _orchardServices.Notifier.Add(NotifyType.Error, T(error));
                            }
                        } else {
                            if (process.Entities.Any(e => !e.Fields.Any(f => f.Input))) {
                                _orchardServices.WorkContext.Resolve<ISchemaHelper>().Help(process);
                            }

                            if (!process.Errors().Any()) {

                                var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                                try {

                                    runner.Execute(process);
                                    process.Request = "Run";
                                    process.Time = timer.ElapsedMilliseconds;

                                    if (process.Errors().Any()) {
                                        foreach (var error in process.Errors()) {
                                            _orchardServices.Notifier.Add(NotifyType.Error, T(error));
                                        }
                                        process.Status = 500;
                                        process.Message = "There are errors in the pipeline.  See log.";
                                    } else {
                                        process.Status = 200;
                                        process.Message = "Ok";
                                    }

                                    var o = process.Output();
                                    switch (o.Provider) {
                                        case "kml":
                                        case "geojson":
                                        case "file":
                                            Response.AddHeader("content-disposition", "attachment; filename=" + o.File);
                                            switch (o.Provider) {
                                                case "kml":
                                                    Response.ContentType = "application/vnd.google-earth.kml+xml";
                                                    break;
                                                case "geojson":
                                                    Response.ContentType = "application/vnd.geo+json";
                                                    break;
                                                default:
                                                    Response.ContentType = "application/csv";
                                                    break;
                                            }
                                            Response.Flush();
                                            Response.End();
                                            return new EmptyResult();
                                        case "excel":
                                            return new FilePathResult(o.File, Common.ExcelContentType) {
                                                FileDownloadName = _slugService.Slugify(part.Title()) + ".xlsx"
                                            };
                                        default:  // page and map are rendered to page
                                            break;
                                    }
                                } catch (Exception ex) {
                                    Logger.Error(ex, ex.Message);
                                    _orchardServices.Notifier.Error(T(ex.Message));
                                }
                            }
                        }
                    }
                } else {
                    _orchardServices.Notifier.Warning(user == "Anonymous" ? T("Sorry. Anonymous users do not have permission to view this report. You may need to login.") : T("Sorry {0}. You do not have permission to view this report.", user));
                }
            }

            return View(new ReportViewModel(process, part));

        }

        private void ConvertToExport(string user, Process process, PipelineConfigurationPart part, string exportType, IDictionary<string, string> parameters) {
            var o = process.Output();
            switch (exportType) {
                case "xlsx":
                    var folder = Common.GetAppFolder();
                    if (!_appDataFolder.DirectoryExists(folder)) {
                        _appDataFolder.CreateDirectory(folder);
                    }

                    var fileName = Common.GetSafeFileName(user, _slugService.Slugify(part.Title()), "xlsx");

                    o.Provider = "excel";
                    o.File = _appDataFolder.MapPath(_appDataFolder.Combine(folder, fileName));
                    break;
                case "geojson":
                    o.Stream = true;
                    o.Provider = "geojson";
                    o.File = _slugService.Slugify(part.Title()) + ".geojson";
                    break;
                case "kml":
                    o.Stream = true;
                    o.Provider = "kml";
                    o.File = _slugService.Slugify(part.Title()) + ".kml";
                    break;
                default: //csv
                    o.Stream = true;
                    o.Provider = "file";
                    o.Delimiter = ",";
                    o.File = _slugService.Slugify(part.Title()) + ".csv";
                    break;
            }

            parameters["page"] = "0";

            foreach (var entity in process.Entities) {

                entity.Page = 0;
                entity.Fields.RemoveAll(f => f.System);

                foreach (var field in entity.GetAllFields()) {
                    if (field.Alias == Common.BatchValueFieldName) {
                        field.Output = false;
                    }
                    field.T = string.Empty; // because short-hand has already been expanded
                    field.Output = field.Output && field.Export == "defer" || field.Export == "true";
                }
            }
        }

        [Themed(false)]
        [HttpGet]
        public ActionResult Download(int id) {

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            var process = new Process { Name = "Export" };

            if (part == null) {
                process.Name = "Not Found";
                return new FileStreamResult(Common.GenerateStreamFromString(process.Serialize()), "text/xml") { FileDownloadName = id + ".xml" };
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                process.Name = "Not Authorized";
                return new FileStreamResult(Common.GenerateStreamFromString(process.Serialize()), "text/xml") { FileDownloadName = id + ".xml" };
            }

            return new FileStreamResult(Common.GenerateStreamFromString(part.Configuration), "text/" + part.EditorMode) { FileDownloadName = _slugService.Slugify(part.Title()) + "." + part.EditorMode };

        }




    }
}