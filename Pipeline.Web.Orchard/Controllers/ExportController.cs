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
using Orchard.Localization;
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
using System.Web.Mvc;
using Transformalize.Contracts;
using Permissions = Orchard.Core.Contents.Permissions;
using Process = Transformalize.Configuration.Process;

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
          ISlugService slugService,
          IAppDataFolder appDataFolder
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

         var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();

         if (part == null) {
            return new HttpNotFoundResult();
         }

         var user = _orchardServices.WorkContext.CurrentUser == null ? "Anonymous" : _orchardServices.WorkContext.CurrentUser.UserName ?? "Anonymous";

         if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
            return new HttpUnauthorizedResult();
         }

         var process = _processService.Resolve(part);

         var parameters = Common.GetParameters(Request, _orchardServices, null);

         process.Load(part.Configuration, parameters);
         process.ReadOnly = true;  // force exporting to omit system fields

         // change process for export and batch purposes
         var reportType = Request["output"] ?? "page";

         ConvertToExport(user, process, part, reportType);
         process.Load(process.Serialize(), parameters);

         Common.SetPageSize(process, parameters, 0, 0, 0);

         if (Request["sort"] != null) {
            _sortService.AddSortToEntity(process.Entities.First(), Request["sort"]);
         }

         if (process.Errors().Any()) {
            foreach (var error in process.Errors()) {
               _orchardServices.Notifier.Add(NotifyType.Error, T(error));
            }
         } else {

            var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();

            var o = process.Output();
            switch (o.Provider) {
               case "kml":
               case "json":
               case "geojson":
               case "file":
                  Response.Clear();
                  Response.BufferOutput = false;

                  switch (o.Provider) {
                     case "kml":
                        Response.ContentType = "application/vnd.google-earth.kml+xml";
                        break;
                     case "geojson":
                     case "json":
                        Response.ContentType = "application/vnd.geo+json";
                        break;
                     default:
                        Response.ContentType = "application/csv";
                        break;
                  }

                  Response.AddHeader("content-disposition", "attachment; filename=" + o.File);
                  runner.Execute(process);
                  return new EmptyResult();
               case "excel":
                  runner.Execute(process);

                  return new FilePathResult(o.File, Common.ExcelContentType) {
                     FileDownloadName = _slugService.Slugify(part.Title()) + ".xlsx"
                  };
               default:  // page and map are rendered to page
                  break;
            }

         }

         return View(new ReportViewModel(process, part));

      }

      private void ConvertToExport(string user, Process process, PipelineConfigurationPart part, string exportType) {
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
               var mapFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                     { part.MapColorField, "geojson-color" },
                     { part.MapPopUpField, "geojson-description" },
                     { part.MapLatitudeField, "latitude" },
                     { part.MapLongitudeField, "longitude" }
                  };
               foreach (var entity in process.Entities) {
                  foreach (var field in entity.GetAllFields()) {
                     field.Output = mapFields.ContainsKey(field.Alias);
                     field.Alias = field.Output ? mapFields[field.Alias] : field.Alias;
                  }
               }
               break;
            case "json":
               o.Stream = true;
               o.Provider = "json";
               o.File = _slugService.Slugify(part.Title()) + ".json";
               break;
            case "calendar":
               o.Stream = true;
               o.Provider = "json";
               o.File = _slugService.Slugify(part.Title()) + ".json";
               var calendarFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                     { part.CalendarIdField, "id" },
                     { part.CalendarTitleField, "title" },
                     { part.CalendarUrlField, "url" },
                     { part.CalendarClassField, "class" },
                     { part.CalendarStartField, "start" },
                     { part.CalendarEndField, "end" },
                  };

               foreach (var entity in process.Entities) {
                  foreach (var field in entity.GetAllFields()) {
                     field.Output = calendarFields.ContainsKey(field.Alias);
                     field.Alias = field.Output ? calendarFields[field.Alias] : field.Alias;
                  }
               }
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

         foreach (var entity in process.Entities) {

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