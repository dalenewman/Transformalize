using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {

    public class AnyTransform : BaseTransform, ITransform {

        class FieldWithValue {
            public Field Field { get; set; }
            public object Value { get; set; }
        }

        private readonly List<FieldWithValue> _input = new List<FieldWithValue>();
        private readonly Func<IRow, bool> _func;

        public AnyTransform(IContext context) : base(context) {
            foreach (var field in MultipleInput()) {
                if (Constants.CanConvert()[field.Type](Context.Transform.Value)) {
                    _input.Add(new FieldWithValue { Field = field, Value = field.Convert(Context.Transform.Value) });
                }
            }

            _func = GetFunc(Context.Transform.Operator);
        }

        /// <summary>
        /// TODO: Implement lessthan,lessthanequal,greaterthan,greaterthanequal
        /// WARNING: Currently only support equal and notequal.
        /// </summary>
        /// <param name="operator"></param>
        /// <returns></returns>
        private Func<IRow, bool> GetFunc(string @operator) {
            // equal,notequal,lessthan,greaterthan,lessthanequal,greaterthanequal,=,==,!=,<,<=,>,>=
            switch (@operator) {
                case "notequal":
                case "!=":
                    return row => _input.Any(f => !row[f.Field].Equals(f.Value));
                case "lessthan":
                case "<":
                case "lessthanequal":
                case "<=":
                case "greaterthan":
                case ">":
                case "greaterthanequal":
                case ">=":
                default:
                    return row => _input.Any(f => row[f.Field].Equals(f.Value));
            }
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = _func(row);
            Increment();
            return row;
        }
    }
}