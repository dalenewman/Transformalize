using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Scripting.CSharp {
    public class CsharpTransform : BaseTransform {
        private readonly CSharpHost.UserCodeInvoker _userCode;

        public CsharpTransform(IContext context) : base(context, null) {
            var name = Utility.GetMethodName(context);

            ConcurrentDictionary<string, CSharpHost.UserCodeInvoker> userCodes;
            if (CSharpHost.Cache.TryGetValue(context.Process.Name, out userCodes)) {
                if (userCodes.TryGetValue(name, out _userCode))
                    return;
            }

            context.Error($"Could not find {name} method in user's code");
            var dv = Constants.TypeDefaults()[context.Field.Type];
            _userCode = objects => dv;
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _userCode(row.ToArray());
            Increment();
            return row;
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            return rows.Select(Transform);
        }
    }
}