using System.Threading;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms.System {
    public class SetKey : BaseTransform {
        private readonly Field _tflKey;

        public SetKey(IContext context) : base(context, null) {
            _tflKey = context.Entity.TflKey();
        }

        public override IRow Transform(IRow row) {
            row[_tflKey] = Interlocked.Increment(ref Context.Entity.Identity);
            // Increment();
            return row;
        }
    }
}