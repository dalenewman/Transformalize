using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class HandsOnTableController : Controller {

        private readonly IOrchardServices _orchardServices;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public HandsOnTableController(IOrchardServices services) {
            _orchardServices = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
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

        public ContentResult Load(int id) {
            //TODO: Have to transform configuration instead of request
            var relayTo = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("Api/Execute", "Api", new { id }) + "?format=json&flavor=arrays";
            Logger.Error(relayTo);
            var content = RelayContent(relayTo);
            return new ContentResult() {
                Content = content,
                ContentType = "application/json"
            };
        }

        public ContentResult Save(int id) {
            //TODO: Have to transform configuration instead of request
            var relayTo = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("Api/Execute", "Api", new { id }) + "?format=json&flavor=arrays";
            Logger.Error(relayTo);
            var content = RelayContent(relayTo);
            return new ContentResult() {
                Content = content,
                ContentType = "application/json"
            };
        }

        private string RelayContent(string url) {

            string content;
            var orchardRequest = Request;
            var serviceRequest = WebRequest.Create(url);

            serviceRequest.Method = orchardRequest.HttpMethod;
            serviceRequest.ContentType = orchardRequest.ContentType;

            if (serviceRequest.Method != "GET") {

                orchardRequest.InputStream.Position = 0;

                var inStream = orchardRequest.InputStream;
                Stream webStream = null;
                try {
                    //copy incoming request body to outgoing request
                    if (inStream != null && inStream.Length > 0) {
                        serviceRequest.ContentLength = inStream.Length;
                        webStream = serviceRequest.GetRequestStream();
                        inStream.CopyTo(webStream);
                    }
                } finally {
                    if (null != webStream) {
                        webStream.Flush();
                        webStream.Close();
                    }
                }
            }

            using (var response = (HttpWebResponse)serviceRequest.GetResponse()) {
                using (var stream = response.GetResponseStream()) {
                    content = new StreamReader(stream).ReadToEnd();
                }
                Response.ContentType = response.ContentType;
            }

            return content;
        }

    }
}