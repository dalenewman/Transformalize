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
using Orchard.ContentManagement;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;
using Pipeline.Web.Orchard.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

      private readonly IOrchardServices _orchardServices;
      private readonly IProcessService _processService;
      private readonly ISecureFileService _secureFileService;

      public MapController(
          IOrchardServices services,
          IProcessService processService,
          ISecureFileService secureFileService
      ) {
         _orchardServices = services;
         _processService = processService;
         _secureFileService = secureFileService;
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

         GetStickyParameters(part.Id, parameters);

         process.Load(part.Configuration, parameters);

         if (process.Errors().Any()) {
            foreach (var error in process.Errors()) {
               _orchardServices.Notifier.Add(NotifyType.Error, T(error));
            }
            return new HttpStatusCodeResult(500, "There are errors in the configuration.");
         }

         MapCfg mapCfg = new MapCfg();
         if (!string.IsNullOrEmpty(part.MapConfiguration)) {
            mapCfg.Load(part.MapConfiguration);
            if (mapCfg.Errors().Any()) {
               foreach (var error in mapCfg.Errors()) {
                  _orchardServices.Notifier.Add(NotifyType.Error, T(error));
               }
               return new HttpStatusCodeResult(500, "There are errors in the map configuration.");
            }
         }

         process.Mode = "map";
         process.ReadOnly = true;  // force maps to omit system fields
         process.Pipeline = "parallel.linq";

         SetStickyParameters(part.Id, process.Parameters);

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
         if (part.MapPaging) {
            sizes.AddRange(part.Sizes(part.MapSizes));
         }

         var stickySize = GetStickyParameter(part.Id, "size", () => sizes.Min());

         Common.SetPageSize(process, parameters, sizes.Min(), stickySize, sizes.Max());

         if (IsMissingRequiredParameters(process.Parameters, _orchardServices.Notifier)) {
            return View(new ReportViewModel(process, part, mapCfg));
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

         return View(new ReportViewModel(process, part, mapCfg));

      }

   }
}