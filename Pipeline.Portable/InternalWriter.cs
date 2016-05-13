using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline {

    public class InternalWriter : IWrite {

        private readonly Entity _entity;

        public InternalWriter(Entity entity) {
            _entity = entity;
        }

        public void Write(IEnumerable<IRow> rows) {
            var fields = _entity.GetAllOutputFields().ToArray();
            foreach (var row in rows) {
                _entity.Rows.Add(row.ToStringDictionary(fields));
            }
        }
    }
}
