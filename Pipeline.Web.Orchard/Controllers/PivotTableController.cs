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

using System.Diagnostics;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Orchard.UI.Notify;
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
    public class PivotTableController : BaseController {

        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        private readonly ISecureFileService _secureFileService;

        public PivotTableController(
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
            _orchardServices = services;
            _processService = processService;
            _secureFileService = secureFileService;
        }

        [Themed(true)]
        public ActionResult Index(int id) {

            var timer = new Stopwatch();
            timer.Start();

            var process = new Process { Name = "PivotTable" };

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                process.Name = "Not Found";
            } else {

                var user = _orchardServices.WorkContext.CurrentUser == null ? "Anonymous" : _orchardServices.WorkContext.CurrentUser.UserName ?? "Anonymous";

                if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                    process = _processService.Resolve(part);

                    var parameters = Common.GetParameters(Request, _orchardServices, _secureFileService);

                    GetStickyParameters(part.Id, parameters);

                    process.Load(part.Configuration, parameters);
                    process.Mode = "pivot";
                    process.ReadOnly = true;  // force pivot to omit system fields

                    SetStickyParameters(part.Id, process.Parameters);

                    // no paging
                    foreach(var entity in process.Entities) {
                        entity.Page = 0;
                    }

                    // no actions
                    process.Actions.Clear();

                    if (IsMissingRequiredParameters(process.Parameters, _orchardServices.Notifier)) {
                        return View(new ReportViewModel(process, part));
                    }

                    // note: currently the view executes the process and converts the rows to a JSON format, 
                    // as opposed to how it works for the map where it initiates a "download"

                } else {
                    _orchardServices.Notifier.Warning(user == "Anonymous" ? T("Sorry. Anonymous users do not have permission to view this report. You may need to login.") : T("Sorry {0}. You do not have permission to view this report.", user));
                }
            }

            return View(new ReportViewModel(process, part));

        }


    }
}