using Transformalize.Model;

namespace Transformalize.Data {
    public interface IEntityBatch {
        int GetNext(Entity entity);
    }
}
