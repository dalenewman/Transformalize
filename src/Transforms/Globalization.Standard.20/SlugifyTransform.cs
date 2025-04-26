#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Globalization {

    public class SlugifyTransform : StringTransform {
        private readonly Field _input;

        public SlugifyTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = Slugify(GetString(row, _input));
            
            return row;
        }

        public static string Slugify(string content) {

            var form = content.ToLower().Normalize(NormalizationForm.FormKD);
            var sb = new StringBuilder();

            foreach (char t in form) {
                // Allowed symbols
                if (t == '-' || t == '_' || t == '~') {
                    sb.Append(t);
                    continue;
                }

                if (t == '\'') {
                    continue;
                }

                var uc = CharUnicodeInfo.GetUnicodeCategory(t);
                switch (uc) {
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.OtherLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        // Keep letters and digits
                        sb.Append(t);
                        break;
                    case UnicodeCategory.NonSpacingMark:
                        // Remove diacritics
                        break;
                    default:
                        // Replace all other chars with dash
                        sb.Append('-');
                        break;
                }
            }

            var slug = sb.ToString().Normalize(NormalizationForm.FormC);

            // Simplifies dash groups 
            for (int i = 0; i < slug.Length - 1; i++) {
                if (slug[i] == '-') {
                    int j = 0;
                    while (i + j + 1 < slug.Length && slug[i + j + 1] == '-') {
                        j++;
                    }
                    if (j > 0) {
                        slug = slug.Remove(i + 1, j);
                    }
                }
            }

            return slug.Trim('-', '_', '.');

        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("slugify") };
        }
    }
}