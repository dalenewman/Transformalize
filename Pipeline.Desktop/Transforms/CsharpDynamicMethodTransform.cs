using System.Collections.Generic;
using Pipeline.Contracts;

namespace Pipeline.Desktop.Transforms {
    public class CsharpDynamicMethodTransform : CSharpBaseTransform {

        private readonly CSharpHost.UserCodeInvoker _userCode;

        public CsharpDynamicMethodTransform(IContext context) : base(context) {

            var name = Pipeline.Utility.GetMethodName(context);
            if (CSharpHost.Cache.TryGetValue(name, out _userCode))
                return;

            context.Error($"Could not find {name} method in user's code");
            var dv = Constants.TypeDefaults()[context.Field.Type];
            _userCode = objects => dv;
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            foreach (var row in rows) {
                row[Context.Field] = _userCode(row.ToArray());
                Increment();
                yield return row;
            }
        }


    }
}