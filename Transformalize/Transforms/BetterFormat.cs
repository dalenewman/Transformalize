#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
    public class BetterFormat {

        private const string Pattern = "(?<={)[^}]+(?=})";
        public readonly Func<IRow, string> Format = row => string.Empty;
        public bool Valid { get; set; }

        public BetterFormat(IContext context, string format, Func<IEnumerable<Field>> fallback) {
            Regex regex = null;
#if NETS10
            regex = new Regex(Pattern);
#else
            regex = new Regex(Pattern, RegexOptions.Compiled);
#endif
            var matches = regex.Matches(format);

            if (matches.Count == 0) {
                Format = row => format;
                Valid = true;
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

            var fields = new List<Field>();
            if (numeric) {
                fields.AddRange(fallback()); // receiving un-named fields
            } else {
                var count = 0;
                foreach (var name in names) {
                    if (context.Entity.TryGetField(name, out var f)) {
                        fields.Add(f);
                        format = format.Replace("{" + name + "}", "{" + count + "}");
                        format = format.Replace("{" + name + ":", "{" + count + ":");
                        count++;
                    } else {
                        context.Error($"Invalid {name} place-holder found in {context.Field.Alias} format template.");
                        return;
                    }
                }
            }

            if (fields.Count < values.Count) {
                context.Error($"Too many place-holders in field {context.Field.Alias}'s format template.");
                return;
            }

            var expanded = new List<Field>(fields.Take(values.Count)).ToArray();

            Format = row => string.Format(format, expanded.Select(f => row[f]).ToArray());
            Valid = true;
        }
    }
}