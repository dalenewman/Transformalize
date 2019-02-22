using System.Linq;
using System.Web;
using Flurl;
using Pipeline.Web.Orchard.Services.Contracts;

namespace Pipeline.Web.Orchard.Services {
    public class LinkService : ILinkService {
        public IHtmlString Create(HttpRequestBase request, string actionUrl, string type) {

            var url = RemoveNoiseFromUrl(request.RawUrl.SetQueryParam("output", type));
            url.Path = actionUrl;

            switch (type) {
                case "map":
                case "report":
                    url.RemoveQueryParam("output");
                    return new HtmlString(url);
                default:
                    return new HtmlString(url.SetQueryParam("output", type).ToString());
            }
            
        }

        private static Url RemoveNoiseFromUrl(Url url) {

            if (url.QueryParams.ContainsKey(Common.InputFileIdName) && url.QueryParams[Common.InputFileIdName].Equals("0")) {
                url.RemoveQueryParam(Common.InputFileIdName);
            }

            var stars = (from param in url.QueryParams where param.Value.Equals("*") || param.Value.Equals("") select param.Name).ToList();
            foreach (var star in stars) {
                url.QueryParams.Remove(star);
            }

            return url;
        }
    }
}