using Orchard;

namespace Pipeline.Web.Orchard.Services {
    public interface ISecureFileService : IDependency {
        FileResponse Get(int id);
    }
}