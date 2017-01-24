using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flurl;

namespace Pipeline.Web.Orchard.Services {
    public class ExportLinkService : IExportLinkService {
        public IHtmlString Create(HttpRequestBase request, string type) {
            var url = request.RawUrl.SetQueryParam("output", type);
            if (url.QueryParams.ContainsKey(Common.InputFileIdName) && url.QueryParams[Common.InputFileIdName].Equals("0")) {
                url.RemoveQueryParam(Common.InputFileIdName);
            }

            var stars = (from param in url.QueryParams where param.Value.Equals("*") || param.Value.Equals("") select param.Name).ToList();
            foreach (var star in stars) {
                url.QueryParams.Remove(star);
            }

            if (type != "page") {
                return new HtmlString(url.SetQueryParam("output", type).ToString());
            }
            return new HtmlString(url.RemoveQueryParam("output").ToString());
        }
    }
}