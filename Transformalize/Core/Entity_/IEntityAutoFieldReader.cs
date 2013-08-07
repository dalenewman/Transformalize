using Transformalize.Core.Fields_;

namespace Transformalize.Core.Entity_ {

    public interface IEntityAutoFieldReader {
        IFields ReadFields();
        IFields ReadPrimaryKey();
        IFields ReadAll();
    }
}
