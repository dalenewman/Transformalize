using Orchard;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface ISecureFileService : IDependency {
        FileResponse Get(int id);
    }
}