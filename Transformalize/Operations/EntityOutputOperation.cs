using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class EntityOutputOperation : AbstractOperation {
        protected List<string> OutputFields { get; set; }

        public EntityOutputOperation(Entity entity) {
            OutputFields = new List<string>();

            foreach (var alias in entity.Fields.WithFileOutput().Aliases()) {
                OutputFields.Add(alias);
            }
            foreach (var alias in entity.CalculatedFields.WithFileOutput().Aliases()) {
                OutputFields.Add(alias);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            throw new System.NotImplementedException();
        }
    }
}