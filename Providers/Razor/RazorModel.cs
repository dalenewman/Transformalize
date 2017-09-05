using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Razor {
    public class RazorModel {
        public RazorModel(Entity entity, IEnumerable<IRow> rows) {
            Entity = entity;
            Rows = rows;
        }
        public Entity Entity { get; set; }
        public IEnumerable<IRow> Rows { get; set; }
    }
}
