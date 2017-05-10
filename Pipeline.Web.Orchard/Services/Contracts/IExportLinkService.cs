using System.Web;
using Orchard;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface IExportLinkService : IDependency {
        IHtmlString Create(HttpRequestBase request, string type);
    }
}