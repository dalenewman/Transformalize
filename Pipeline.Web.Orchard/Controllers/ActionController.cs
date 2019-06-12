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
using Orchard.Localization;
using Orchard.Logging;
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
   public class ActionController : BaseController {

      private readonly IOrchardServices _orchardServices;
      private readonly IProcessService _processService;
      private readonly ISecureFileService _secureFileService;
      private readonly IBatchCreateService _batchCreateService;
      private readonly IBatchWriteService _batchWriteService;
      private readonly IBatchRunService _batchRunService;
      private readonly IBatchRedirectService _batchRedirectService;

      public ActionController(
          IOrchardServices services,
          IProcessService processService,
          ISecureFileService secureFileService,
          IBatchCreateService batchCreateService,
          IBatchWriteService batchWriteService,
          IBatchRunService batchRunService,
          IBatchRedirectService batchRedirectService
      ) {
         _orchardServices = services;
         _processService = processService;
         _secureFileService = secureFileService;
         _batchCreateService = batchCreateService;
         _batchWriteService = batchWriteService;
         _batchRunService = batchRunService;
         _batchRedirectService = batchRedirectService;
         T = NullLocalizer.Instance;
         Logger = NullLogger.Instance;
      }

      [Themed(false), HttpPost]
      public ActionResult Index(int id) {

         var timer = new Stopwatch();
         timer.Start();

         var referrer = HttpContext.Request.UrlReferrer == null ? Url.Action("Index", "Report", new { Id = id }) : HttpContext.Request.UrlReferrer.ToString();
         var process = new Process { Name = "Action" };

         var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
         if (part == null) {
            process.Name = "Not Found";
            _orchardServices.Notifier.Add(NotifyType.Error, T("Could not find pipeline id {0}", id));
            return _batchRedirectService.Redirect(referrer, null);
         }

         var user = _orchardServices.WorkContext.CurrentUser == null ? "Anonymous" : _orchardServices.WorkContext.CurrentUser.UserName ?? "Anonymous";

         if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {

            process = _processService.Resolve(part);

            var parameters = Common.GetParameters(Request, _orchardServices, _secureFileService);
            if (part.NeedsInputFile && Convert.ToInt32(parameters[Common.InputFileIdName]) == 0) {
               _orchardServices.Notifier.Add(NotifyType.Error, T("This transformalize expects a file."));
               process.Name = "File Not Found";
            }

            process.Load(part.Configuration, parameters);
            process.ReadOnly = true;  // force actions to omit system fields

            if (parameters.ContainsKey("action")) {

               var action = process.Actions.FirstOrDefault(a => a.Description == parameters["action"]);
               var system = process.Parameters.FirstOrDefault(p => p.Name.Equals("System", StringComparison.OrdinalIgnoreCase));

               if (system == null) {
                  Logger.Error("The {0} requires a System parameter in order to run bulk actions.", part.Title());
                  _orchardServices.Notifier.Error(T("The {0} requires a System parameter in order to run bulk actions.", part.Title()));
               } else {
                  if (system.Value == "*") {
                     _orchardServices.Notifier.Warning(T("The {0} must be selected in order to run an action.", system.Label));
                     system = null;
                  }
               }

               if (action != null && system != null) {

                  // check security
                  var actionPart = _orchardServices.ContentManager.Get(action.Id);
                  if (actionPart == null) {
                     _orchardServices.Notifier.Add(NotifyType.Error, T("Could not find orchard content id {0} referenced in the {1}:{2} action.", id, action.Type, action.Description));
                     return _batchRedirectService.Redirect(referrer, null);
                  }

                  if (_orchardServices.Authorizer.Authorize(Permissions.ViewContent, actionPart)) {

                     parameters["entity"] = process.Entities.First().Alias;

                     var batchParameters = _batchCreateService.Create(process, parameters);
                     batchParameters["Orchard.User"] = parameters["Orchard.User"];
                     batchParameters["count"] = parameters.ContainsKey("count") ? parameters["count"] : "0";

                     var count = _batchWriteService.Write(Request, process, batchParameters);

                     if (count > 0) {

                        // get default parameters from process
                        foreach (var p in process.Parameters.Where(pr => pr.Value != "*")) {
                           if (!batchParameters.ContainsKey(p.Name)) {
                              batchParameters[p.Name] = p.Value;
                           }
                        }

                        // get parameters from action
                        foreach (var p in action.Parameters.Where(pr => pr.Value != "*")) {
                           if (batchParameters.ContainsKey(p.Name)) {
                              _orchardServices.Notifier.Add(NotifyType.Warning, T("The action parameter {0} is hiding another parameter", p.Name));
                           }
                           batchParameters[p.Name] = p.Value;
                        }

                        if (_batchRunService.Run(action, batchParameters)) {
                           if (action.Url == string.Empty) {
                              if (batchParameters.ContainsKey("BatchId")) {
                                 _orchardServices.Notifier.Information(T(string.Format("Processed {0} records in batch {1}.", count, batchParameters["BatchId"])));
                              } else {
                                 _orchardServices.Notifier.Information(T(string.Format("Processed {0} records.", count)));
                              }
                              return _batchRedirectService.Redirect(referrer, null);
                           }
                           return _batchRedirectService.Redirect(action.Url, batchParameters);
                        }

                        var message = batchParameters.ContainsKey("BatchId") ? string.Format("Batch {0} failed.", batchParameters["BatchId"]) : "Batch failed.";
                        Logger.Error(message);
                        _orchardServices.Notifier.Error(T(message));
                        foreach (var key in batchParameters.Keys) {
                           Logger.Error("Batch Parameter {0} = {1}.", key, batchParameters[key]);
                        }
                        return _batchRedirectService.Redirect(referrer, null);
                     }
                  } else {
                     return new HttpUnauthorizedResult("You do not have access to this bulk action.");
                  }
               }

            } else {
               _orchardServices.Notifier.Warning(T("An action must be specified."));
            }

         } else {
            _orchardServices.Notifier.Warning(user == "Anonymous" ? T("Sorry. Anonymous users do not have permission to view this report. You may need to login.") : T("Sorry {0}. You do not have permission to view this report.", user));
         }

         return _batchRedirectService.Redirect(referrer, null);

      }

   }
}