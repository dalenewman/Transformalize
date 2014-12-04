using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Transformalize.Main;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class HandsOnTableController : Controller {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransformalizeService _transformalize;
        private readonly IApiService _apiService;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly NameValueCollection _query = new NameValueCollection(2);

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public HandsOnTableController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IApiService apiService
        ) {
            _orchardServices = services;
            _transformalize = transformalize;
            _apiService = apiService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _query.Add("format", "json");
            _query.Add("flavor", "arrays");
            _stopwatch.Start();
        }

        public ActionResult Index(int id) {
            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
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

            return View(part);
        }

        public ActionResult Load(int id) {
            return ModifyAndRun(id, TransformConfigurationForLoad);
        }

        public ActionResult Save(int id) {
            _query.Add("data", Request.Form["data"]);
            return ModifyAndRun(id, s => s);
        }

        private static string TransformConfigurationForLoad(string configuration) {
            // ReSharper disable PossibleNullReferenceException
            var xml = XDocument.Parse(configuration).Root;

            var process = xml.Element("processes").Element("add");
            process.SetAttributeValue("star-enabled", "false");

            SwapInputAndOutput(ref process);

            var entity = process.Element("entities").Element("add");
            var fields = entity.Elements("fields").Elements("add").ToArray();

            if (!fields.Any(f => f.Attribute("primary-key") != null && f.Attribute("primary-key").Value.Equals("true", IC))) {
                foreach (var field in fields) {
                    field.SetAttributeValue("primary-key", "true");
                }
            }

            entity.SetAttributeValue("detect-changes", "false");
            if (entity.Attributes("delete").Any() && entity.Attribute("delete").Value.Equals("true", IC)) {
                entity.Attributes("delete").Remove();

                var filter = new XElement("filter");
                var add = new XElement("add");
                add.SetAttributeValue("left", "TflDeleted");
                add.SetAttributeValue("right", "0");
                filter.Add(add);
                entity.Add(filter);

                var lastField = fields.Last();
                var deleted = new XElement("add");
                deleted.SetAttributeValue("name", "TflDeleted");
                deleted.SetAttributeValue("type", "boolean");
                deleted.SetAttributeValue("output", "false");
                deleted.SetAttributeValue("label", "Deleted");
                lastField.AddAfterSelf(deleted);
            }

            return xml.ToString();
            // ReSharper restore PossibleNullReferenceException
        }

        private static void SwapInputAndOutput(ref XElement process) {
            var connections = process.Element("connections").Elements().ToArray();
            connections.First(c => c.Attribute("name").Value.Equals("output")).SetAttributeValue("name", "temp");
            connections.First(c => c.Attribute("name").Value.Equals("input")).SetAttributeValue("name", "output");
            connections.First(c => c.Attribute("name").Value.Equals("temp")).SetAttributeValue("name", "input");
        }

        private ActionResult ModifyAndRun(int id, Func<string, string> modifier) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            var request = new ApiRequest(ApiRequestType.Execute) { Stopwatch = _stopwatch };

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return _apiService.NotFound(request, _query);
            }

            if (User.Identity.IsAuthenticated) {
                if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                    return _apiService.Unathorized(request, _query);
                }
            } else {
                return _apiService.Unathorized(request, _query);
            }

            var transformalizeRequest = new TransformalizeRequest();
            try {
                transformalizeRequest = new TransformalizeRequest {
                    Configuration = modifier(part.Configuration),
                    Options = new Options(),
                    Query = _query
                };
            } catch (Exception ex) {
                request.Status = 500;
                request.Message = ex.Message;
            }

            var processes = new TransformalizeResponse();

            try {
                processes = _transformalize.Run(transformalizeRequest);
            } catch (Exception ex) {
                request.Status = 500;
                request.Message = ex.Message;
            }

            return new ApiResponse(request, transformalizeRequest.Configuration, processes).ContentResult(_query["format"], _query["flavor"]);

        }



    }
}