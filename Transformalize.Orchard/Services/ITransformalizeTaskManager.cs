using Orchard;
using Orchard.ContentManagement;

namespace Transformalize.Orchard.Services {
    public interface ITransformalizeTaskManager : IDependency {
        void CreateTask(ContentItem item);
    }
}