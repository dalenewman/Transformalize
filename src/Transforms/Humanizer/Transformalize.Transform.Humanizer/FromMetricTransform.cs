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
using Humanizer;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Humanizer {
    public class FromMetricTransform : BaseTransform {

        private readonly Func<IRow, object> _transform;
        private readonly HashSet<string> _warnings = new HashSet<string>();

        public FromMetricTransform(IContext context = null) : base(context, "double") {
            if (IsMissingContext()) {
                return;
            }
            if (IsNotReceiving("string")) {
                return;
            }

            var input = SingleInput();

            _transform = (row) => {
                var value = (string)row[input];
                if (!IsInvalidMetricNumeral(value))
                    return Context.Field.Convert(value.FromMetric());

                var warning = $"The value {value} is an invalid metric numeral.";
                if (_warnings.Add(warning)) {
                    Context.Warn(warning);
                }
                var numbers = GetNumbers(value);
                return Context.Field.Convert(numbers.Length > 0 ? numbers : "0");
            };
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);

            return row;
        }

        public override void Dispose() {
            _warnings.Clear();
        }

        private static string GetNumbers(string input) {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        // The following code is from Humanizer
        // Humanizer is by Alois de Gouvello https://github.com/aloisdg
        // The MIT License (MIT)
        // Copyright (c) 2015 Alois de Gouvello

        private static readonly List<char>[] Symbols = {
            new List<char> { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' },
            new List<char> { 'm', 'μ', 'n', 'p', 'f', 'a', 'z', 'y' }
        };


        private static readonly Dictionary<char, string> Names = new Dictionary<char, string> {
            {'Y', "yotta" }, {'Z', "zetta" }, {'E', "exa" }, {'P', "peta" }, {'T', "tera" }, {'G', "giga" }, {'M', "mega" }, {'k', "kilo" },
            {'m', "milli" }, {'μ', "micro" }, {'n', "nano" }, {'p', "pico" }, {'f', "femto" }, {'a', "atto" }, {'z', "zepto" }, {'y', "yocto" }
        };

        private static string ReplaceNameBySymbol(string input) {
            return Names.Aggregate(input, (current, name) =>
                current.Replace(name.Value, name.Key.ToString()));
        }

        private static bool IsInvalidMetricNumeral(string input) {
            input = input.Trim();
            input = ReplaceNameBySymbol(input);

            var index = input.Length - 1;
            var last = input[index];
            var isSymbol = Symbols[0].Contains(last) || Symbols[1].Contains(last);
            return !double.TryParse(isSymbol ? input.Remove(index) : input, out var number);
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("frommetric") };
        }
    }
}