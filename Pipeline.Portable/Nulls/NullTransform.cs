using Pipeline.Contracts;

namespace Pipeline.Nulls {
    public class NullTransform : ITransform {
        public IContext Context { get; }

        public NullTransform(IContext context) {
            Context = context;
        }
        public IRow Transform(IRow row) {
            return row;
        }
    }
}