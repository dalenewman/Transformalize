using System;
using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {

    public enum FilterType {
        Exclude,
        Include
    }

    public class FilterTransform : BaseTransform {
        private readonly Func<IRow, bool> _filter;
        private readonly FilterType _filterType;

        public FilterTransform(IContext context, FilterType filterType) : base(context, null) {
            _filterType = filterType;
            _filter = GetFunc(SingleInput(), context.Transform.Operator, SingleInput().Convert(context.Transform.Value));
        }

        public override IRow Transform(IRow row) {
            throw new NotImplementedException();
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            return _filterType == FilterType.Include ? rows.Where(row => _filter(row)) : rows.Where(row => !_filter(row));
        }

        public static Func<IRow, bool> GetFunc(Field input, string @operator, object value) {
            // equal,notequal,lessthan,greaterthan,lessthanequal,greaterthanequal,=,==,!=,<,<=,>,>=
            switch (@operator) {
                case "notequal":
                case "!=":
                    return row => row[input].Equals(value);
                case "lessthan":
                case "<":
                    return row => ((IComparable)row[input]).CompareTo(value) < 0;
                case "lessthanequal":
                case "<=":
                    return row => ((IComparable)row[input]).CompareTo(value) < 0;
                case "greaterthan":
                case ">":
                    return row => ((IComparable)row[input]).CompareTo(value) > 0;
                case "greaterthanequal":
                case ">=":
                    return row => ((IComparable)row[input]).CompareTo(value) >= 0;
                default:
                    return row => row[input].Equals(value);
            }
        }
    }
}