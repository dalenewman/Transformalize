using System.Web;
using Orchard;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface ILinkService : IDependency {
        IHtmlString Create(HttpRequestBase request, HttpSessionStateBase session, int partId, string actionUrl, string type, bool everything);
    }
}