using System.Collections.Generic;

namespace Pipeline.Contracts {
    public interface ITransformRows {
        IEnumerable<IRow> Transform(IEnumerable<IRow> rows);
    }

    public class NullTransformRows : ITransformRows {
        public IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            return rows;
        }
    }
}