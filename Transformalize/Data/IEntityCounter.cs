using Transformalize.Model;

namespace Transformalize.Data {
    public interface IEntityCounter {
        int CountInput(Entity entity);
        int CountOutput(Entity entity);
    }
}
