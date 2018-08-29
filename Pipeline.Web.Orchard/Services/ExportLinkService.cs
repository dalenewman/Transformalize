using System.Linq;
using System.Web;
using Flurl;
using Pipeline.Web.Orchard.Services.Contracts;

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

            if (type == "map") {
                url.Path = url.Path.Replace("/Report", "/Map");
                url.RemoveQueryParam("output");
                return new HtmlString(url);
            }

            if (type == "report") {
                url.Path = url.Path.Replace("/Map", "/Report");
                url.RemoveQueryParam("output");
                return new HtmlString(url);
            }

            return new HtmlString(url.SetQueryParam("output", type).ToString());
        }
    }
}