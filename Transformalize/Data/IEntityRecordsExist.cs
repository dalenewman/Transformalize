using Transformalize.Model;

namespace Transformalize.Data {
    public interface IEntityRecordsExist {
        bool OutputRecordsExist(Entity entity);
        bool InputRecordsExist(Entity entity);
    }
}