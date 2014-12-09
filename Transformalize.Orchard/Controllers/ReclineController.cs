using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Transformalize.Logging;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class ReclineController : WidgetController {

        private readonly IOrchardServices _orchardServices;
        private readonly IApiService _apiService;
        private readonly NameValueCollection _query = new NameValueCollection(2);

        public ReclineController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IApiService apiService
        )
            : base(services, transformalize, apiService) {
            _orchardServices = services;
            _apiService = apiService;
            _query.Add("format", "json");
            _query.Add("flavor", "objects");
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

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            ConfigurationPart part;
            ApiRequest request;

            foreach (var rejection in _apiService.Rejections(id, out request, out part)) {
                return rejection.ContentResult(_query["format"], _query["flavor"]);
            }

            var xml = XDocument.Parse(part.Configuration).Root;
            var process = xml.Element("processes").Element("add");

            var entity = process.Element("entities").Element("add");
            var fields = entity.Elements("fields").Elements("add").ToArray();

            DefaultAttributesIfMissing(fields, "output","true");
            var modified = xml.ToString();

            request.RequestType = ApiRequestType.Execute;
            
            return Run(request, part, _query, modified).ContentResult(_query["format"], _query["flavor"]);
        }

    }
}