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
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Pipeline.Contracts;
using Pipeline.Web.Orchard.Services;
using Pipeline.Web.Orchard.Models;
using Process = Pipeline.Configuration.Process;
using Permissions = global::Orchard.Core.Contents.Permissions;

namespace Pipeline.Web.Orchard.Controllers {

    [ValidateInput(false), Themed(false)]
    public class ApiController : Controller {

        private static readonly HashSet<string> _formats = new HashSet<string> { "xml", "json", "yaml" };

        readonly IOrchardServices _orchardServices;
        readonly IIpRangeService _ipRangeService;
        private readonly IProcessService _processService;
        private readonly ISortService _sortService;
        private readonly IFileService _fileService;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ApiController(
            IOrchardServices orchardServices,
            IIpRangeService ipRangeService,
            IProcessService processService,
            ISortService sortService,
            IFileService fileService
        ) {
            _orchardServices = orchardServices;
            _ipRangeService = ipRangeService;
            _processService = processService;
            _fileService = fileService;
            _sortService = sortService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        [ActionName("Api/Run")]
        public ContentResult Run(int id) {

            const string action = "Run";
            var timer = new Stopwatch();
            timer.Start();
            var format = GetFormat(Request);

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            if (part == null) {
                return Get404(action, _processService, format);
            }

            format = GetFormat(Request, part);

            if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

                var process = _processService.Resolve(part.EditorMode, format);
                var parameters = Common.GetParameters(Request);
                process.Load(part.Configuration, parameters);

                if (process.Errors().Any()) {
                    return Get503(action, process, format, timer.ElapsedMilliseconds);
                }

                Common.PageHelper(process, Request);
                
                if (MissingFieldHelper(process, part, format, parameters)) {
                    if (process.Errors().Any()) {
                        return Get503(action, process, format, timer.ElapsedMilliseconds);
                    }
                }

                var sort = Request["sort"];
                if (!string.IsNullOrEmpty(sort)) {
                    _sortService.AddSortToEntity(process.Entities.First(), sort);
                }

                var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                try {
                    runner.Execute(process);
                    process.Status = 200;
                    process.Message = "Ok";
                    process.Request = action;
                    process.Time = timer.ElapsedMilliseconds;
                    RemoveCredentials(process);
                    RemoveShorthand(process);
                    if (process.Output().IsInternal()) {
                        RemoveSystemFields(process);
                    }
                    return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
                } catch (Exception ex) {
                    return Get501(Request, _orchardServices, action, ex.Message, timer.ElapsedMilliseconds);
                }

            }

            return Get401(format, _orchardServices, action);

        }

        [ActionName("Api/Cfg")]
        public ContentResult Configuration(int id) {
            Response.AddHeader("Access-Control-Allow-Origin", "*");

            const string action = "Cfg";

            var timer = new Stopwatch();
            timer.Start();
            //var format = GetFormat(Request);

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            if (part == null) {
                return Get404(action, _processService, "xml");
            }

            if (_ipRangeService.InRange(Request.UserHostAddress, part.StartAddress, part.EndAddress)) {
                return new ContentResult { Content = part.Configuration, ContentType = "text/" + part.EditorMode };
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                return Get401(action, _orchardServices, part.EditorMode);
            }

            return new ContentResult { Content = part.Configuration, ContentType = "text/" + part.EditorMode };

            /* can't do this until i move the "adaptors" out of process validate or entity validate
             var process = _processService.Resolve(part.EditorMode, format, pass:true);  // a pass does not process place-holders or short-hand
             process.Load(part.Configuration);
             process.Request = action;
             process.Status = 200;
             process.Time = timer.ElapsedMilliseconds;  // not including cost of serialize
             process.Message = "Ok";
             RemoveCredentials(process);
             RemoveSystemFields(process);
             return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
             */
        }

        [ActionName("Api/Check")]
        public ContentResult Validate(int id) {

            const string action = "Check";
            var timer = new Stopwatch();
            timer.Start();

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            var format = GetFormat(Request);

            if (part == null) {
                timer.Stop();
                return Get404(action, _processService, format, timer.ElapsedMilliseconds);
            }

            format = GetFormat(Request, part);

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                return Get401(action, _orchardServices, format);
            }

            var process = _processService.Resolve(part.EditorMode, format);
            var parameters = Common.GetParameters(Request);
            process.Load(part.Configuration, parameters);

            if (process.Errors().Any()) {
                return Get503(action, process, format, timer.ElapsedMilliseconds);
            }

            if (MissingFieldHelper(process, part, format, parameters)) {
                if (process.Errors().Any()) {
                    return Get503(action, process, format, timer.ElapsedMilliseconds);
                }
            }

            RemoveCredentials(process);
            RemoveShorthand(process);

            var sort = Request["sort"];
            if (!string.IsNullOrEmpty(sort)) {
                _sortService.AddSortToEntity(process.Entities.First(), sort);
            }

            process.Request = action;
            process.Status = 200;
            process.Time = timer.ElapsedMilliseconds;  // not including cost of serialize
            process.Message = "Ok";


            return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
        }

        private bool MissingFieldHelper(Process process, PipelineConfigurationPart part, string format, IDictionary<string, string> parameters) {
            if (process.Entities.Any(e => !e.Fields.Any(f => f.Input))) {
                var schemaHelper = _orchardServices.WorkContext.Resolve<ISchemaHelper>();
                if (schemaHelper.Help(process)) {
                    if (part.EditorMode == format) {
                        process.Load(process.Serialize(), parameters);
                    } else {
                        var cfg = process.Serialize();
                        process = _processService.Resolve(format, format);
                        process.Load(cfg);
                    }
                    return true;
                }
            }
            return false;
        }

        private static void RemoveCredentials(Process process) {
            foreach (var connection in process.Connections) {
                connection.ConnectionString = string.Empty;
                connection.User = string.Empty;
                connection.Password = string.Empty;
            }
        }

        private static void RemoveSystemFields(Process process) {
            foreach (var entity in process.Entities) {
                entity.Fields.RemoveAll(f => f.System);
            }
        }

        private static void RemoveShorthand(Process process) {
            foreach (var field in process.GetAllFields().Where(f => !string.IsNullOrEmpty(f.T))) {
                field.T = string.Empty;
            }
        }

        private static ContentResult Get404(string action, IProcessService service, string format, long time = 5) {
            var process = service.Resolve("xml", format);
            process.Request = action;
            process.Status = 404;
            process.Message = "Configuration not found.";
            process.Time = time;
            return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
        }

        private static ContentResult Get401(string action, IOrchardServices services, string format, long time = 5) {
            var process = format == "json" ? (Process)services.WorkContext.Resolve<JsonProcess>() : services.WorkContext.Resolve<XmlProcess>();
            process.Request = action;
            process.Status = 401;
            process.Message = "Unauthorized";
            process.Time = time;
            return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
        }

        private static ContentResult Get503(string action, Process process, string format, long time) {
            process.Request = action;
            process.Status = 503;
            process.Message = string.Join("\n", process.Errors());
            process.Time = time;
            return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
        }

        private static ContentResult Get501(HttpRequestBase request, IOrchardServices services, string action, string message, long time = 5) {
            var format = request.QueryString["format"] == "json" ? "json" : "xml";
            var process = format == "json" ? (Process)services.WorkContext.Resolve<JsonProcess>() : services.WorkContext.Resolve<XmlProcess>();
            process.Request = action;
            process.Status = 501;
            process.Message = message;
            process.Time = time;
            return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
        }

        private static string GetFormat(HttpRequestBase request, PipelineConfigurationPart part = null) {
            var value = request.QueryString["format"];
            if (value == null) {
                return part == null ? "xml" : part.EditorMode;
            }
            value = value.ToLower();
            return _formats.Contains(value) ? value : "xml";
        }
    }
}