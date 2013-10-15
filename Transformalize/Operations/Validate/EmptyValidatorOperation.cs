using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Validate
{
    public class EmptyValidatorOperation : AbstractOperation {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return rows;
        }
    }
}