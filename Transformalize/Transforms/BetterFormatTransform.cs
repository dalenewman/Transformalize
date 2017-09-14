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
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class BetterFormatTransform : BaseTransform {
        private readonly Field[] _input;
        private const string Pattern = "(?<={)[^}]+(?=})";
        private readonly Func<IRow, string> _transform;

        public BetterFormatTransform(IContext context) : base(context, "string") {

            _transform = row => context.Operation.Format;
            Regex regex = null;
#if NETS10
            regex = new Regex(Pattern);
#else
            regex = new Regex(Pattern, RegexOptions.Compiled);
#endif
            var matches = regex.Matches(context.Operation.Format);

            if (matches.Count == 0) {
                Error($"a format transform in {Context.Field.Alias} is missing place-holders.");
                return;
            }

            var values = new List<string>(); // using list to maintain insertion order (HashSet<string> does not)
            foreach (Match match in matches) {
                if (!values.Contains(match.Value)) {
                    values.Add(match.Value);
                }
            }

            var numeric = true;
            var names = new List<string>();

            foreach (var value in values) {
                var left = value.Split(':')[0];
                if (left.ToCharArray().All(c => c >= '0' && c <= '9'))
                    continue;
                if (!names.Contains(left)) {
                    names.Add(left);
                }
                numeric = false;
            }

            if (numeric) {
                _input = MultipleInput();  // receiving fields from parameters (or copu(x,y,etc))
            } else {
                var fields = new List<Field>();
                var count = 0;
                foreach (var name in names) {
                    Field field;
                    if (context.Process.TryGetField(name, out field)) {
                        fields.Add(field);
                        context.Operation.Format = context.Operation.Format.Replace("{" + name, "{" + count);
                        count++;
                    } else {
                        Error($"Invalid field name {name} found in a format transform in field {Context.Field.Alias}.");
                        return;
                    }
                }
                _input = fields.ToArray();
            }


            if (values.Count != _input.Length) {
                Error($"The number of supplied fields does not match the number of place-holders in a format transform in field {Context.Field.Alias}.");
                return;
            }

            _transform = row => string.Format(Context.Operation.Format, _input.Select(f => row[f]).ToArray());
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }

    }
}