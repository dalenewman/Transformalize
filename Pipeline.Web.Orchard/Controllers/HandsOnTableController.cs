using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Services;
using Orchard.Themes;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Logging;

namespace Pipeline.Web.Orchard.Controllers {

   [Themed, SessionState(SessionStateBehavior.ReadOnly)]
   public class HandsOnTableController : WidgetController {

      private readonly IJsonConverter _jsonConverter;
      private readonly IOrchardServices _orchardServices;
      private readonly IProcessService _processService;
      private readonly Dictionary<string, string> _query = new Dictionary<string, string>(3, StringComparer.OrdinalIgnoreCase);

      public HandsOnTableController(IOrchardServices services, IProcessService processService, IJsonConverter jsonConverter) {
         _jsonConverter = jsonConverter;
         _orchardServices = services;
         _processService = processService;
         _query.Add("format", "json");
         _query.Add("mode", "default");
      }

      public ActionResult Index(int id) {
         var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
         if (part == null) {
            return new HttpNotFoundResult();
         }

         if (User.Identity.IsAuthenticated) {
            if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
               return new HttpUnauthorizedResult();
            }
         } else {
            System.Web.Security.FormsAuthentication.RedirectToLoginPage();
         }

         var showEditButton = _orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.EditContent, part);
         return View(new HandsOnTableViewModel { Part = part, ShowEditButton = showEditButton });
      }

      [Themed(false)]
      public ActionResult Load(int id) {
         Response.AddHeader("Access-Control-Allow-Origin", "*");

         if (!User.Identity.IsAuthenticated) {
            return new HttpUnauthorizedResult();
         }

         var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
         if (part == null) {
            return new HttpNotFoundResult();
         }

         if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
            return new HttpUnauthorizedResult();
         }

         Response.ContentType = "application/json";

         var process = _processService.Resolve(part, part.EditorMode, "json");
         process.Load(part.Configuration, _query);

         var errors = process.Errors();
         if (errors.Any()) {
            return new HttpStatusCodeResult(500, string.Join(" ", errors));
         }

         TransformConfigurationForLoad(process);

         var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
         runner.Execute(process);

         // because dates are not formatted nice for JSON automatically
         foreach(var entity in process.Entities) {
            foreach(var field in entity.Fields.Where(f=>f.Type.StartsWith("date") && f.Format != string.Empty)) {
               foreach(var row in entity.Rows) {
                  row[field.Alias] = Convert.ToDateTime(row[field.Alias]).ToString(field.Format);
               }
            }
         }

         process.Status = process.Log.Any(l => l.Level == "error") ? (short)500 : (short)200;

         return new ContentResult { Content = process.Serialize() };
      }

      [Themed(false)]
      public ActionResult Save(int id) {
         Response.AddHeader("Access-Control-Allow-Origin", "*");

         if (!User.Identity.IsAuthenticated) {
            return new HttpUnauthorizedResult();
         }

         var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
         if (part == null) {
            return new HttpNotFoundResult();
         }

         if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
            return new HttpUnauthorizedResult();
         }

         var process = _processService.Resolve(part, part.EditorMode, "json");
         process.Load(part.Configuration, _query);

         if (!string.IsNullOrEmpty(Request.Form["data"])) {
            var data = _jsonConverter.Deserialize(Request.Form["data"]);
            Response.ContentType = "application/json";
            var inputFields = process.Entities[0].Fields.Where(f => f.Input).ToArray();
            var inputFieldNames = inputFields.Select(f => f.Name).ToArray();
            foreach (var item in data.rows) {
               var row = new CfgRow(inputFieldNames);
               for (var i = 0; i < inputFields.Length; i++) {
                  var field = inputFields[i];
                  try {
                     var value = field.Convert(item[i].Value);
                     row[field.Name] = value;
                  } catch (Exception) {
                     process.Log.Add(new LogEntry(LogLevel.Error,null,"Trouble converting field {0}'s value {1} to data type {2}.",field.Alias, item[i].Value, field.Type));
                     return new ContentResult { Content = process.Serialize() };
                  }
               }
               process.Entities[0].Rows.Add(row);
            }
            var runner = _orchardServices.WorkContext.Resolve<IRunTimeExecute>();
            runner.Execute(process);
            process.Status = process.Log.Any(l => l.Level == "error") ? (short)500 : (short)200;
            return new ContentResult { Content = process.Serialize() };
         }

         return new HttpStatusCodeResult(500);
      }

   }
}