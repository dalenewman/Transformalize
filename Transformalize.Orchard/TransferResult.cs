using System;
using System.Web;
using System.Web.Mvc;

namespace Transformalize.Orchard {

    public class TransferResult : ActionResult {
        public string Url { get; private set; }

        public TransferResult(string url) {
            this.Url = url;
        }

        public override void ExecuteResult(ControllerContext context) {
            if (context == null)
                throw new ArgumentNullException("context");

            var httpContext = HttpContext.Current;

            // MVC 3 running on IIS 7+
            if (HttpRuntime.UsingIntegratedPipeline) {
                httpContext.Server.TransferRequest(this.Url, true);
            } else {
                // Pre MVC 3
                httpContext.RewritePath(this.Url, false);
                IHttpHandler httpHandler = new MvcHttpHandler();
                httpHandler.ProcessRequest(httpContext);
            }
        }
    }
}