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
using System.Text.RegularExpressions;
using Humanizer;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Humanizer {
    public class FromRomanTransform : BaseTransform {
        private static readonly Regex ValidRomanNumeral = new Regex("^(?i:(?=[MDCLXVI])((M{0,3})((C[DM])|(D?C{0,3}))?((X[LC])|(L?XX{0,2})|L)?((I[VX])|(V?(II{0,2}))|V)?))$", RegexOptions.Compiled);
        private readonly Func<IRow, object> _transform;
        private readonly Field _input;
        private readonly HashSet<string> _warnings = new HashSet<string>();

        public FromRomanTransform(IContext context = null) : base(context, "int") {
            if (IsMissingContext()) {
                return;
            }
            if (IsNotReceiving("string")) {
                return;
            }

            _input = SingleInput();
            switch (_input.Type) {
                case "string":
                _transform = (row) => {
                    var input = ((string)row[_input]).Trim();

                    if (input.Length == 0 || IsInvalidRomanNumeral(input)) {
                        var warning = $"The input {input} is an invalid roman numeral";
                        if (_warnings.Add(warning)) {
                            Context.Warn(warning);
                        }
                        return Context.Field.Convert("0");
                    }
                    return Context.Field.Convert(input.FromRoman());
                };
                break;
                default:
                _transform = (row) => row[_input];
                break;
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);

            return row;
        }


        // The following code is from Humanizer
        // Humanizer is by Alois de Gouvello https://github.com/aloisdg
        // The MIT License (MIT)
        // Copyright (c) 2015 Alois de Gouvello

        private static bool IsInvalidRomanNumeral(string input) {
            return !ValidRomanNumeral.IsMatch(input);
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("fromroman") };
        }
    }
}