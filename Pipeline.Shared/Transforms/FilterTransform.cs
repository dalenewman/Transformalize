using System;
using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FilterTransform : BaseTransform {
        private readonly Field _input;
        private Func<IRow, bool> _filter;

        public FilterTransform(IContext context) : base(context) {
            _input = SingleInput();
            _filter = GetFunc(context.Transform.Operator, _input.Convert(context.Transform.Value));
        }

        public override IRow Transform(IRow row) {
            throw new NotImplementedException();
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            return rows.Where(row => !_filter(row));
        }

        private Func<IRow, bool> GetFunc(string @operator, object value) {
            // equal,notequal,lessthan,greaterthan,lessthanequal,greaterthanequal,=,==,!=,<,<=,>,>=
            switch (@operator) {
                case "notequal":
                case "!=":
                    return row => !row[_input].Equals(value);
                case "lessthan":
                case "<":
                    return row => ((IComparable)row[_input]).CompareTo(value) < 0;
                case "lessthanequal":
                case "<=":
                    return row => ((IComparable)row[_input]).CompareTo(value) < 0;
                case "greaterthan":
                case ">":
                    return row => ((IComparable)row[_input]).CompareTo(value) > 0;
                case "greaterthanequal":
                case ">=":
                    return row => ((IComparable)row[_input]).CompareTo(value) >= 0;
                default:
                    return row => row[_input].Equals(value);
            }
        }
    }
}