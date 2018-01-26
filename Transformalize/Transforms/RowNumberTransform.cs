using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class RowNumberTransform : BaseTransform {
        private int _rowNumber;

        public RowNumberTransform(IContext context = null) : base(context, "int") {
            IsMissingContext();
        }

        public override IRow Operate(IRow row) {
            ++_rowNumber;
            row[Context.Field] = _rowNumber;
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("rownumber") };
        }
    }
}