using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class OppositeTransform : BaseTransform {

        private readonly Field _input;
        private readonly Func<object, object> _transform;

        public OppositeTransform(IContext context = null) : base(context, "object") {
            if (IsMissingContext()) {
                return;
            }

            var type = Received();

            Returns = type; // since you don't know when constructing

            switch (type) {
                case "float":
                case "single":
                    _transform = n => (float)n * -1;
                    break;
                case "double":
                    _transform = n => (double)n * -1;
                    break;
                case "decimal":
                    _transform = n => (decimal)n * -1.0M;
                    break;
                case "int16":
                case "short":
                    _transform = n => (short)n * -1;
                    break;
                case "int64":
                case "long":
                    _transform = n => (long) n * -1;
                    break;
                case "int32":
                case "int":
                    _transform = n => (int)n * -1;
                    break;
                case "bool":
                case "boolean":
                    _transform = n => !(bool)n;
                    break;
                default:
                    Context.Warn($"Invalid attempt opposite an {type} type in {Context.Field.Alias}!");
                    _transform = n => n;
                    Run = false;
                    return;
            }

            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row[_input]);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("opposite") };
        }
    }
}