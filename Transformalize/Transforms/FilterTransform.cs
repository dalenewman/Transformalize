#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public enum FilterType {
        Exclude,
        Include
    }

    public class FilterTransform : BaseTransform {
        private readonly Func<IRow, bool> _filter;
        private readonly FilterType _filterType;

        public FilterTransform(IContext context, FilterType filterType) : base(context, null) {
            if (IsMissing(context.Operation.Operator)) {
                return;
            }
            if (IsMissing(context.Operation.Value)) {
                return;
            }

            var input = SingleInput();

            if (input.Type == "byte[]") {
                Error($"The {context.Operation.Method} method doesn't work with byte arrays.");
                Run = false;
                return;
            }

            if (input.Type != "string" && !Constants.CanConvert()[input.Type](context.Operation.Value)) {
                Error($"The {context.Operation.Method} method's value of {context.Operation.Value} can't be converted to a {input.Type} for comparison.");
                Run = false;
                return;
            }

            _filterType = filterType;
            _filter = GetFunc(input, context.Operation.Operator, SingleInput().Convert(context.Operation.Value));
        }

        public override IRow Operate(IRow row) {
            throw new NotImplementedException();
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            return _filterType == FilterType.Include ? rows.Where(row => _filter(row)) : rows.Where(row => !_filter(row));
        }

        public static Func<IRow, bool> GetFunc(Field input, string @operator, object value) {
            // equal,notequal,lessthan,greaterthan,lessthanequal,greaterthanequal,=,==,!=,<,<=,>,>=
            switch (@operator) {
                case "notequal":
                case "neq":
                case "!=":
                    return row => row[input].Equals(value);
                case "lessthan":
                case "lt":
                case "<":
                    return row => ((IComparable)row[input]).CompareTo(value) < 0;
                case "lessthanequal":
                case "lte":
                case "<=":
                    return row => ((IComparable)row[input]).CompareTo(value) < 0;
                case "greaterthan":
                case "gt":
                case ">":
                    return row => ((IComparable)row[input]).CompareTo(value) > 0;
                case "greaterthanequal":
                case "gte":
                case ">=":
                    return row => ((IComparable)row[input]).CompareTo(value) >= 0;
                default:
                    return row => row[input].Equals(value);
            }
        }
    }
}