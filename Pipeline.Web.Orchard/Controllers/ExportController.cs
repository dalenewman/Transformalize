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
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
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
using Permissions = Orchard.Core.Contents.Permissions;

namespace Pipeline.Web.Orchard.Controllers {

    [ValidateInput(false), Themed]
    public class ExportController : Controller {

        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        private readonly ISortService _sortService;
        private readonly ISlugService _slugService;
        private readonly IAppDataFolder _appDataFolder;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ExportController(
            IOrchardServices services,
            IProcessService processService,
            ISortService sortService,
            ICfgService cfgService,
            ISlugService slugService,
            IAppDataFolder appDataFolder,
            IClock clock
        ) {
            _appDataFolder = appDataFolder;
            _orchardServices = services;
            _processService = processService;
            _sortService = sortService;
            _slugService = slugService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        [Themed(true)]
        public ActionResult Index(int id) {

            var timer = new Stopwatch();
            timer.Start();

            var process = new Process { Name = "Export" };

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                process.Name = "Not Found";
            } else {

                var user = _orchardServices.WorkContext.CurrentUser == null ? "Anonymous" : _orchardServices.WorkContext.CurrentUser.UserName ?? "Anonymous";

                if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                    process = _processService.Resolve(part);

                    var parameters = Common.GetParameters(Request, _orchardServices, null);

                    process.Load(part.Configuration, parameters);
                    process.Buffer = false; // no buffering for export
                    process.ReadOnly = true;  // force exporting to omit system fields

                    // change process for export and batch purposes
                    var reportType = Request["output"] ?? "page";

                    ConvertToExport(user, process, part, reportType, parameters);
                    process.Load(process.Serialize(), parameters);

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
                                process.Request = "Export";
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
                default: // csv
                    o.Stream = true;
                    o.Provider = "file";
                    o.Delimiter = ",";
                    o.TextQualifier = "\"";
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

    }
}