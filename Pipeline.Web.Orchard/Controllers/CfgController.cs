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
using Cfg.Net.Ext;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using Pipeline.Contracts;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;
using System.IO;
using Orchard.Autoroute.Services;
using Orchard.FileSystems.AppData;
using Orchard.Services;
using Pipeline.Extensions;
using Process = Pipeline.Configuration.Process;

namespace Pipeline.Web.Orchard.Controllers {

    [ValidateInput(false), Themed(true)]
    public class CfgController : Controller {

        const string FileTimestamp = "yyyy-MM-dd-HH-mm-ss";
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        private readonly ISortService _sortService;
        private readonly ISecureFileService _secureFileService;
        private readonly ICfgService _cfgService;
        private readonly ISlugService _slugService;
        private readonly IAppDataFolder _appDataFolder;
        private readonly IClock _clock;
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
            IClock clock
            ) {
            _clock = clock;
            _appDataFolder = appDataFolder;
            _orchardServices = services;
            _processService = processService;
            _secureFileService = secureFileService;
            _cfgService = cfgService;
            _sortService = sortService;
            _slugService = slugService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

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

        public ActionResult Report(int id) {

            var timer = new Stopwatch();
            timer.Start();

            var process = new Process { Name = "Report" }.WithDefaults();

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                process.Name = "Not Found";
            } else {
                if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                    process = _processService.Resolve(part.EditorMode, part.EditorMode);

                    var parameters = Common.GetParameters(Request, _secureFileService, _orchardServices);
                    if (part.NeedsInputFile && Convert.ToInt32(parameters[Common.InputFileIdName]) == 0) {
                        _orchardServices.Notifier.Add(NotifyType.Error, T("This transformalize expects a file."));
                        process.Name = "File Not Found";
                    }

                    process.Load(part.Configuration, parameters);

                    var provider = process.Output().Provider;
                    if (provider.In("internal", "file")) {

                        // change process for export purposes
                        var output = Request["output"] ?? "page";
                        if (part.Reportable && output != "page") {
                            ConvertToExport(process, part, output, parameters);
                            process.Load(process.Serialize(), parameters);
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
                                Common.PageHelper(process, parameters);
                                var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                                try {
                                    runner.Execute(process);
                                    process.Status = 200;
                                    process.Message = "Ok";
                                    process.Request = "Run";
                                    process.Time = timer.ElapsedMilliseconds;
                                    var o = process.Output();
                                    if (o.Provider == "file") {
                                        Response.AddHeader("content-disposition", "attachment; filename=" + o.File);
                                        Response.ContentType = output == "xlsx" ? ExcelContentType : "application/csv";
                                        Response.Flush();
                                        Response.End();
                                    } else if (process.Output().Provider == "excel") {
                                        return new FilePathResult(o.File, ExcelContentType) {
                                            FileDownloadName = _slugService.Slugify(part.Title()) + ".xlsx"
                                        };
                                    }
                                } catch (Exception ex) {
                                    Logger.Error(ex, ex.Message);
                                    _orchardServices.Notifier.Error(T(ex.Message));
                                }
                            }
                        }
                    }
                } else {
                    _orchardServices.Notifier.Warning(T("Output must be set to internal for reporting."));
                }
            }

            return View(new ReportViewModel(process, part));

        }

        private void ConvertToExport(Process process, PipelineConfigurationPart part, string output, IDictionary<string,string> parameters) {
            var o = process.Output();
            switch (output) {
                case "xlsx":
                    if (!_appDataFolder.DirectoryExists(Common.FileFolder)) {
                        _appDataFolder.CreateDirectory(Common.FileFolder);
                    }

                    var user = _orchardServices.WorkContext.CurrentUser == null ? 
                        "Anonymous" : 
                        _orchardServices.WorkContext.CurrentUser.UserName ?? "Anonymous";

                    var fileName = $"{user}-{_clock.UtcNow.ToString(FileTimestamp)}-{_slugService.Slugify(part.Title())}.xlsx";
                    
                    o.Provider = "excel";
                    o.File = _appDataFolder.MapPath(_appDataFolder.Combine(Common.FileFolder,fileName));
                    break;
                default: //csv
                    o.Provider = "file";
                    o.Delimiter = ",";
                    o.File = _slugService.Slugify(part.Title()) + ".csv";
                    break;
            }

            parameters["page"] = "0";
            foreach (var entity in process.Entities) {
                entity.Page = 0;
                foreach (var field in entity.GetAllFields().Where(f=>!f.System)) {
                    field.T = string.Empty;
                    if (field.Output && field.Transforms.Any()) {
                        var lastTransform = field.Transforms.Last();
                        if (lastTransform.Method == "tag" || lastTransform.Method == "razor" && field.Raw) {
                            var firstParameter = lastTransform.Parameters.First();
                            field.Transforms.Remove(lastTransform);
                            field.T = "copy(" + firstParameter.Field + ")";
                        }
                    }
                }
            }
        }

        [Themed(false)]
        [HttpGet]
        public ActionResult Download(int id) {

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            var process = new Configuration.Process { Name = "Export" }.WithDefaults();

            if (part == null) {
                process.Name = "Not Found";
                return new FileStreamResult(GenerateStreamFromString(process.Serialize()), "text/xml") { FileDownloadName = id + ".xml" };
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                process.Name = "Not Authorized";
                return new FileStreamResult(GenerateStreamFromString(process.Serialize()), "text/xml") { FileDownloadName = id + ".xml" };
            }

            return new FileStreamResult(GenerateStreamFromString(part.Configuration), "text/" + part.EditorMode) { FileDownloadName = _slugService.Slugify(part.Title()) + "." + part.EditorMode };

        }


        public Stream GenerateStreamFromString(string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


    }
}