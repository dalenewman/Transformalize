using System.Collections.Specialized;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class HandsOnTableController : WidgetController {

        private readonly IOrchardServices _orchardServices;
        private readonly NameValueCollection _query = new NameValueCollection(2);

        public HandsOnTableController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IApiService apiService
        )
            : base(services, transformalize, apiService) {
            _orchardServices = services;
            _query.Add("format", "json");
            _query.Add("flavor", "arrays");
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
            return ModifyAndRun(id, TransformConfigurationForLoad, _query);
        }

        public ActionResult Save(int id) {
            _query.Add("data", Request.Form["data"]);
            return ModifyAndRun(id, s => s, _query);
        }

    }
}