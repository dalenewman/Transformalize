using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Processes
{
    public class EntityOutputOperation : AbstractOperation {
        protected List<string> OutputFields { get; set; }

        public EntityOutputOperation(Entity entity) {
            OutputFields = new List<string>();

            foreach (var pair in entity.Fields.Where(f => f.Value.FileOutput)) {
                OutputFields.Add(pair.Value.Alias);
            }
            foreach (var pair in entity.CalculatedFields.Where(f => f.Value.FileOutput)) {
                OutputFields.Add(pair.Value.Alias);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            throw new System.NotImplementedException();
        }
    }
}