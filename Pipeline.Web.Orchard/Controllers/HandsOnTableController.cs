using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Services;
using Orchard.Themes;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;
using Transformalize.Contracts;
using Transformalize.Impl;

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
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            }

            var showEditButton = _orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.EditContent, part);
            return View(new HandsOnTableViewModel { Part = part, ShowEditButton = showEditButton});
        }

        [Themed(false)]
        public ActionResult Load(int id) {
            Response.AddHeader("Access-Control-Allow-Origin", "*");

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                return new HttpNotFoundResult();
            }

            if (User.Identity.IsAuthenticated) {
                if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                    return new HttpUnauthorizedResult();
                }
            } else {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
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
            process.Status = process.Log.Any(l => l.Level == "error") ? (short)500 : (short)200;

            return new ContentResult { Content = process.Serialize() };
        }

        [Themed(false)]
        public ActionResult Save(int id) {
            Response.AddHeader("Access-Control-Allow-Origin", "*");

            var part = _orchardServices.ContentManager.Get(id).As<PipelineConfigurationPart>();
            if (part == null) {
                return new HttpNotFoundResult();
            }

            if (User.Identity.IsAuthenticated) {
                if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                    return new HttpUnauthorizedResult();
                }
            } else {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
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
                        var value = item[i].ToString();
                        row[field.Name] = value;
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