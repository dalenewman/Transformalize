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
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Cfg.Net.Ext;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using Pipeline.Contracts;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;
using Permissions = global::Orchard.Core.Contents.Permissions;

namespace Pipeline.Web.Orchard.Controllers {

    [ValidateInput(false), Themed(true)]
    public class ReportController : Controller {

        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        private readonly ISortService _sortService;
        private readonly ISecureFileService _secureFileService;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ReportController(
            IOrchardServices services,
            IProcessService processService,
            ISortService sortService,
            ISecureFileService secureFileService
            ) {
            _orchardServices = services;
            _processService = processService;
            _secureFileService = secureFileService;
            _sortService = sortService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        [Themed]
        public ActionResult Report(int id) {

            var timer = new Stopwatch();
            timer.Start();

            var process = new Configuration.Process { Name = "Report" }.WithDefaults();

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                process.Name = "Not Found";
            } else {
                if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                    process = _processService.Resolve(part.EditorMode, part.EditorMode);
                    var parameters = Common.GetParameters(Request, _secureFileService);
                    if (part.NeedsInputFile && Convert.ToInt32(parameters[Common.InputFileIdName]) == 0) {
                        _orchardServices.Notifier.Add(NotifyType.Error, T("This transformalize expects a file."));
                        process.Name = "File Not Found";
                    }

                    process.Load(part.Configuration, parameters);

                    if (Request["sort"] != null) {
                        _sortService.AddSortToEntity(process.Entities.First(), Request["sort"]);
                    }

                    if (process.Output().IsInternal()) {
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
                                } catch (Exception ex) {
                                    Logger.Error(ex, ex.Message);
                                    _orchardServices.Notifier.Error(T(ex.Message));
                                }
                            }

                        }
                    } else {
                        _orchardServices.Notifier.Warning(T("Output must be set to internal for reporting."));
                    }
                }
            }

            return View(process);

        }
    }
}