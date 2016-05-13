using System.Collections.Generic;
using System.Linq;
using Pipeline.Contracts;

namespace Pipeline.Nulls {

    public class NullDeleteHandler : IEntityDeleteHandler {
        public IEnumerable<IRow> DetermineDeletes() {
            return Enumerable.Empty<IRow>();
        }

        public void Delete() {

        }
    }
}