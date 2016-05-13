using Pipeline.Contracts;

namespace Pipeline.Nulls {
    public class NullRowCondition : IRowCondition {
        public bool Eval(IRow row) {
            return true;
        }
    }
}