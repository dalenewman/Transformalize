using System.Linq;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class ConnectionTransform : BaseTransform, ITransform {

        private readonly object _value;
        public ConnectionTransform(IContext context) : base(context) {
            var connection = context.Process.Connections.First(c => c.Name == context.Transform.Name);
            _value = Utility.GetPropValue(connection, context.Transform.Property);
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = _value;
            Increment();
            return row;
        }
    }
}