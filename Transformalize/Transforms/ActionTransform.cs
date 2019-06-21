using Cfg.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Transformalize.Contracts;
using Action = Transformalize.Configuration.Action;

namespace Transformalize.Transforms {
    public class ActionTransform : BaseTransform {

        private readonly string[] _props;
        private readonly Action _action;

        public ActionTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.Property)) {
                return;
            }

#if NETS10
            _props = typeof(Action).GetRuntimeProperties().Where(prop => CustomAttributeExtensions.GetCustomAttribute((MemberInfo)prop, typeof(CfgAttribute), (bool)true) != null).Select(prop => prop.Name).ToArray();
#else
            _props = typeof(Action).GetProperties().Where(prop => prop.GetCustomAttributes(typeof(CfgAttribute), true).FirstOrDefault() != null).Select(prop => prop.Name).ToArray();
#endif

            var set = new HashSet<string>(_props, StringComparer.OrdinalIgnoreCase);

            if (!set.Contains(Context.Operation.Property)) {
                Error($"The action property {Context.Operation.Property} is not allowed.  The allowed properties are {(string.Join(", ", _props))}.");
                Run = false;
                return;
            }

            Context.Operation.Property = set.First(s => s.Equals(Context.Operation.Property, StringComparison.OrdinalIgnoreCase));

            if (Context.Operation.Index < Context.Process.Actions.Count) {
                _action = Context.Process.Actions[Context.Operation.Index];
            } else {
                Run = false;
                Context.Error($"action index {Context.Operation.Index} is out of bounds");
            }

        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = Utility.GetPropValue(_action, Context.Operation.Property);
            return row;
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            foreach (var row in rows) {
                row[Context.Field] = Utility.GetPropValue(_action, Context.Operation.Property);
                yield return row;
            }
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("action") {
                Parameters = new List<OperationParameter>(2){
                    new OperationParameter("index","0"),
                    new OperationParameter("property")
                }
            };
        }
    }
}