using Transformalize.Contracts;

namespace Transformalize.Transforms.System {
    public class LogTransform : BaseTransform {
 
        public LogTransform(IContext context = null) : base(context, "null") {
            IsMissingContext();
        }

        public override IRow Operate(IRow row) {
            if (Context.Entity.RowNumber % Context.Entity.LogInterval == 0) {
                Context.Info($"{Context.Entity.RowNumber} rows transformed.");
            }
            return row;
        }

    }
}