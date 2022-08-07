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
using Transformalize.Contracts;

namespace Transformalize {

    public static class ColumnNames {

        public static IEnumerable<string> Generate(int count) {
            for (var i = 0; i < count; i++) {
                yield return Utility.GetExcelName(i);
            }
        }

        public static bool AreValid(IContext context, params string[] names) {
            var valid = true;

            var duplicates = Duplicates(names).ToArray();
            if (duplicates.Any()) {
                foreach (var value in duplicates) {
                    context.Warn($"Column header {value} occurs more than once.");
                }
                valid = false;
            }

            if (ContainEmptyOrWhiteSpace(names)) {
                context.Warn("One of the potential column names is completely blank.");
                valid = false;
            }

            if (ContainNumber(names)) {
                context.Warn("One of the potential column names is numeric.");
                valid = false;
            }

            if (ContainDatetime(names)) {
                context.Warn("One of the potential column names is a date.");
                valid = false;
            }

            if (ContainGuid(names)) {
                context.Warn("One of the potential columns names is a guid.");
                valid = false;
            }

            return valid;
        }

        public static IEnumerable<string> Duplicates(IEnumerable<string> names) {
            return names.GroupBy(n => n.ToLower()).Where(n => n.Count() > 1).Select(n => n.Key).Distinct();
        }

        private static bool ContainEmptyOrWhiteSpace(IEnumerable<string> names) {
            return names.Any(n => string.IsNullOrEmpty(n) || string.IsNullOrWhiteSpace(n));
        }

        private static bool ContainNumber(IEnumerable<string> names) {
            float number;
            return names.Any(n => float.TryParse(n, out number));
        }

        private static bool ContainDatetime(IEnumerable<string> names) {
            DateTime date;
            return names.Any(n => DateTime.TryParse(n, out date));
        }

        private static bool ContainGuid(IEnumerable<string> names) {
            Guid guid;
            return names.Any(n => Guid.TryParse(n, out guid));
        }
    }
}
