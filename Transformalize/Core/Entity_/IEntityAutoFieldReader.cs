using Transformalize.Core.Fields_;

namespace Transformalize.Core.Entity_ {

    public interface IEntityAutoFieldReader {
        IFields Read(Entity entity, bool isMaster);
    }
}
