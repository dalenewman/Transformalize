using Transformalize.Contracts;

namespace Transformalize.Transforms.System {
    public class IncrementTransform : BaseTransform {
        public IncrementTransform(IContext context) : base(context, "null") {
            IsMissingContext();
        }

        public override IRow Operate(IRow row) {
            ++Context.Entity.RowNumber;
            return row;
        }
    }
}