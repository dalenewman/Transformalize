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
using Transformalize.Contracts;
using Transformalize.Extensions;
using Pipeline.Web.Orchard.Services;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services.Contracts;
using System.Web.SessionState;
using LogLevel = Transformalize.Contracts.LogLevel;
using Process = Transformalize.Configuration.Process;
using Permissions = Orchard.Core.Contents.Permissions;

namespace Pipeline.Web.Orchard.Controllers {

    [ValidateInput(false), Themed(false), SessionState(SessionStateBehavior.ReadOnly)]
    public class ApiController : Controller {

        private static readonly HashSet<string> _formats = new
        HashSet<string> { "xml", "json", "yaml" };

        private readonly IOrchardServices _orchardServices;
        private readonly IIpRangeService _ipRangeService;
        private readonly IProcessService _processService;
        private readonly ISortService _sortService;
        private readonly ISecureFileService _secureFileService;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ApiController(
            IOrchardServices orchardServices,
            IIpRangeService ipRangeService,
            IProcessService processService,
            ISortService sortService,
            ISecureFileService secureFileService
        ) {
            _orchardServices = orchardServices;
            _ipRangeService = ipRangeService;
            _processService = processService;
            _sortService = sortService;
            _secureFileService = secureFileService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ActionResult Run(int id) {

            const string action = "Run";
            var timer = new Stopwatch();
            timer.Start();
            var format = GetFormat(Request);

            Response.AddHeader("Access-Control-Allow-Origin", "*");

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            if (part == null) {
                Logger.Warning("Request from {0} for missing id {1}.", Request.UserHostAddress, id);
                return Get404(action, format);
            }

            format = GetFormat(Request, part);
            var authorized = false;

            if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                Logger.Debug("Authorization granted to {0} for {1}.", User.Identity.Name, part.Title());
                authorized = true;
            } else {
                // You can "run" a transformalize job it's tagged as SERVICE and has an allowed IP Range
                if (!string.IsNullOrEmpty(part.StartAddress) && part.Tags().Contains("SERVICE", StringComparer.OrdinalIgnoreCase)) {
                    if (_ipRangeService.InRange(Request.UserHostAddress, part.StartAddress, part.EndAddress)) {
                        Logger.Information("Service authorization granted to {0} for {1}.", Request.UserHostAddress, part.Title());
                        authorized = true;
                    } else {
                        Logger.Warning("Service authorization denied to {0} for {1}.", Request.UserHostAddress, part.Title());
                    }
                } else {
                    if (User.Identity.IsAuthenticated) {
                        Logger.Warning("Authorization denied to {0} for {1}.", User.Identity.Name, part.Title());
                    } else {
                        Logger.Warning("Authorization denied to anonymous user at {0} for {1}.", Request.UserHostAddress, part.Title());
                    }
                }
            }

            if (authorized) {

                var process = _processService.Resolve(part, part.EditorMode, format);
                var parameters = Common.GetParameters(Request, _orchardServices, _secureFileService);

                process.Load(part.Configuration, parameters);

                if (process.Errors().Any()) {
                    Logger.Error("Configuration {0} has errors: {1}", id, string.Join(" ", process.Errors()));
                    return Get503(action, process, format, timer.ElapsedMilliseconds);
                }

                Common.TranslatePageParametersToEntities(process, parameters, "api");

                if (MissingFieldHelper(process, part, format, parameters)) {
                    if (process.Errors().Any()) {
                        Logger.Error("Configuration from missing fields {0} has errors: {1}", id, string.Join(" ", process.Errors()));

                        return Get503(action, process, format, timer.ElapsedMilliseconds);
                    }
                }

                var sort = Request["sort"];
                if (!string.IsNullOrEmpty(sort)) {
                    _sortService.AddSortToEntity(process.Entities.First(), sort);
                }

                var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
                try {

                    var output = process.Output();

                    runner.Execute(process);

                    if (process.Log.Any()) {
                        process.Status = process.Log.Any(le => le.LogLevel == LogLevel.Error) ? (short)500 : (short)200;
                        process.Message = string.Format("{0} error{1} and/or warning{1} recorded.", process.Log.Count, process.Log.Count.Plural());
                    } else {
                        if (output.Provider == "text") {
                            Response.Flush();
                            Response.End();
                        }
                        process.Status = 200;
                        process.Message = "Ok";
                    }
                    process.Request = action;
                    process.Time = timer.ElapsedMilliseconds;
                    RemoveCredentials(process);

                    //var returnUrl = (Request.Form["ReturnUrl"] ?? Request.QueryString["ReturnUrl"]) ?? string.Empty;
                    //if (!returnUrl.Equals(string.Empty))
                    //    return new RedirectResult(returnUrl);
                    return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
                } catch (Exception ex) {
                    Logger.Error(ex, "Executing {0} threw error: {1}", id, ex.Message);

                    return Get501(Request, _processService, action, ex.Message, timer.ElapsedMilliseconds);
                }

            }

            Logger.Warning("Unathorized user {0} attempting access to {1}.", User.Identity.IsAuthenticated ? User.Identity.Name : "Anonymous@" + Request.UserHostAddress, id);


            return Get401(format, action);

        }

        public ContentResult Cfg(int id) {
            Response.AddHeader("Access-Control-Allow-Origin", "*");

            const string action = "Cfg";

            var timer = new Stopwatch();
            timer.Start();

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            if (part == null) {
                return Get404(action, "xml");
            }

            if (_ipRangeService.InRange(Request.UserHostAddress, part.StartAddress, part.EndAddress)) {
                return new ContentResult { Content = part.Configuration, ContentType = "text/" + part.EditorMode };
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                return Get401(action, part.EditorMode);
            }

            return new ContentResult { Content = part.Configuration, ContentType = "text/" + part.EditorMode };

        }

        public ContentResult Check(int id) {

            const string action = "Check";
            var timer = new Stopwatch();
            timer.Start();

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

            var format = GetFormat(Request);

            if (part == null) {
                timer.Stop();
                return Get404(action, format, timer.ElapsedMilliseconds);
            }

            format = GetFormat(Request, part);

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                return Get401(action, format);
            }

            var process = _processService.Resolve(part, part.EditorMode, format);
            var parameters = Common.GetParameters(Request, _orchardServices, _secureFileService);

            if (part.NeedsInputFile && Convert.ToInt32(parameters[Common.InputFileIdName]) == 0) {
                return GetStatus(404, "need input file", action, format);
            }

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

                    // remove this stuff before serialization
                    // todo: get clean, unmodified, and unvalidated configuration to to add fields to and serialize
                    // because below won't work in all cases (i.e. producer transforms...)
                    foreach (var entity in process.Entities) {
                        entity.Fields.RemoveAll(f => f.System);
                    }

                    foreach (var field in process.GetAllFields().Where(f => !string.IsNullOrEmpty(f.T))) {
                        field.T = string.Empty;
                    }

                    if (part.EditorMode == format) {
                        process.Load(process.Serialize(), parameters);
                    } else {
                        var cfg = process.Serialize();
                        process = _processService.Resolve(part, format, format);
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
            foreach (var parameter in process.Environments.SelectMany(environement => environement.Parameters)) {
                if (parameter.Name.Equals("user", StringComparison.OrdinalIgnoreCase)) {
                    parameter.Value = string.Empty;
                }
                if (parameter.Name.Equals("password")) {
                    parameter.Value = string.Empty;
                }
            }
        }

        private static ContentResult Get404(string action, string format, long time = 5) {
            var message = format == "json" ? string.Format("{{ \"request\":\"{0}\", \"status\":404, \"message\":\"not found\", \"time\":{1} }}", action, time)
                : string.Format("<cfg request=\"{0}\" status=\"404\" message=\"not found\" time=\"{1}\" />", action, time);
            return new ContentResult {
                Content = message,
                ContentType = "text/" + format
            };
        }

        private static ContentResult Get401(string action, string format, long time = 5) {
            var message = format == "json" ? string.Format("{{ \"request\":\"{0}\", \"status\":401, \"message\":\"not allowed\", \"time\":{1} }}", action, time) : string.Format("<cfg request=\"{0}\" status=\"401\" message=\"not allowed\" time=\"{1}\" />", action, time);
            return new ContentResult {
                Content = message,
                ContentType = "text/" + format
            };
        }

        private static ContentResult GetStatus(int status, string message, string action, string format, long time = 5) {
            var msg = format == "json" ? string.Format("{{ \"request\":\"{0}\", \"status\":{1}, \"message\":\"{2}\", \"time\":{3} }}", action, status, message, time)
                : string.Format("<cfg request=\"{0}\" status=\"{1}\" message=\"{2}\" time=\"{3}\" />", action, status, message, time);
            return new ContentResult { Content = msg, ContentType = "text/" + format };
        }


        private static ContentResult Get503(string action, Process process, string format, long time) {
            process.Request = action;
            process.Status = 503;
            process.Message = string.Join("\n", process.Errors());
            process.Time = time;
            return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
        }

        private static ContentResult Get501(HttpRequestBase request, IProcessService service, string action, string message, long time = 5) {
            var format = request.QueryString["format"] == "json" ? "json" : "xml";
            var process = service.Resolve(new PipelineConfigurationPart(), format, format);
            process.Request = action;
            process.Status = 501;
            process.Message = message;
            process.Time = time;
            return new ContentResult { Content = process.Serialize(), ContentType = "text/" + format };
        }

        private static string GetFormat(HttpRequestBase request, PipelineConfigurationPart part = null) {
            var value = request.QueryString["format"];
            if (value == null) {
                value = request.Form["format"];
                if (value == null) {
                    return part == null ? "xml" : part.EditorMode;
                }
            }
            value = value.ToLower();
            return _formats.Contains(value) ? value : "xml";
        }
    }
}