using Cfg.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class ParameterTransform : BaseTransform {

        private readonly string[] _props;
        private readonly Parameter _parameter;

        public ParameterTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.Name)) {
                return;
            }

            if (IsMissing(Context.Operation.Property)) {
                return;
            }

#if NETS10
            _props = typeof(Parameter).GetRuntimeProperties().Where(prop => CustomAttributeExtensions.GetCustomAttribute((MemberInfo)prop, typeof(CfgAttribute), (bool)true) != null).Select(prop => prop.Name).ToArray();
#else
            _props = typeof(Parameter).GetProperties().Where(prop => prop.GetCustomAttributes(typeof(CfgAttribute), true).FirstOrDefault() != null).Select(prop => prop.Name).ToArray();
#endif

            var set = new HashSet<string>(_props, StringComparer.OrdinalIgnoreCase);

            if (!set.Contains(Context.Operation.Property)) {
                Error($"The parameter property {Context.Operation.Property} is not allowed.  The allowed properties are {(string.Join(", ", _props))}.");
                Run = false;
                return;
            }

            Context.Operation.Property = set.First(s => s.Equals(Context.Operation.Property, StringComparison.OrdinalIgnoreCase));

            _parameter = Context.Process.Parameters.First(c => c.Name.Equals(Context.Operation.Name, StringComparison.OrdinalIgnoreCase));
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = Utility.GetPropValue(_parameter, Context.Operation.Property);
            return row;
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            foreach (var row in rows) {
                row[Context.Field] = Utility.GetPropValue(_parameter, Context.Operation.Property);
                yield return row;
            }
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("parameter") {
                Parameters = new List<OperationParameter>(2){
                    new OperationParameter("name"),
                    new OperationParameter("property")
                }
            };
        }
    }
}