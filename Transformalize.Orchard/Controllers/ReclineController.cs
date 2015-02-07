using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Transformalize.Configuration;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class ReclineController : WidgetController {

        private readonly IOrchardServices _orchardServices;
        private readonly IApiService _apiService;
        private readonly Dictionary<string,string> _query = new Dictionary<string, string>(3);

        public ReclineController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IApiService apiService
        )
            : base(transformalize) {
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

            var root = new TflRoot(part.Configuration, _query);

            var problems = root.Problems();
            if (problems.Any()) {
                request.Status = 500;
                request.Message = string.Join(Environment.NewLine, problems);
                return new ApiResponse(request, root.ToString(), new TransformalizeResponse()).ContentResult(_query["format"], _query["flavor"]);
            }

            request.RequestType = ApiRequestType.Execute;
            var transformalizeRequest = new TransformalizeRequest(part, _query, null, root);

            return Run(request, transformalizeRequest).ContentResult(_query["format"], _query["flavor"]);
        }

    }
}