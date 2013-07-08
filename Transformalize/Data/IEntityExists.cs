using Transformalize.Model;

namespace Transformalize.Data {
    public interface IEntityExists {
        bool OutputExists(Entity entity);
        bool InputExists(Entity entity);
    }
}