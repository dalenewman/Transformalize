using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Nulls {
    public class NullInputProvider : IInputProvider {
        public object GetMaxVersion() {
            return null;
        }

        public Schema GetSchema(Entity entity = null) {
            return new Schema();
        }

        public IEnumerable<IRow> Read() {
            return Enumerable.Empty<IRow>();
        }
    }
}
