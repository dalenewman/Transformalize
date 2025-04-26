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
using System;
using System.Collections.Generic;
using System.Globalization;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class ConvertTransform : BaseTransform {

        private readonly Func<object, object> _convert;
        private readonly string _type;

        public ConvertTransform(IContext context = null) : base(context, (context == null ? "object" : (context.Operation.Type == Constants.DefaultSetting ? context.Field.Type : context.Operation.Type))) {
            if (IsMissingContext()) {
                return;
            }

            _type = Constants.TypeSet().Contains(Context.Operation.Type) ? Context.Operation.Type : Context.Field.Type;

            if (_type.StartsWith("date") && Context.Operation.Format != string.Empty) {
                _convert = (v) => DateTime.ParseExact(v.ToString(), Context.Operation.Format, CultureInfo.InvariantCulture);
            } else {
                _convert = (v) => Constants.ObjectConversionMap[_type](v);
            }
        }

        public override IRow Operate(IRow row) {
            throw new NotImplementedException("This shouldn't be called directly.");
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            var tried = false;
            var input = SingleInput();
            foreach (var row in rows) {
                if (tried) {
                    row[Context.Field] = _convert(row[input]);
                } else {
                    try {
                        row[Context.Field] = _convert(row[input]);
                        tried = true;
                    } catch (Exception) {
                        Context.Error($"Couldn't convert {row[input]} to {_type}.");
                        yield break;
                    }
                }

                yield return row;
            }

        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("convert") {
                Parameters = new List<OperationParameter>(2){
                    new OperationParameter("type", Constants.DefaultSetting),
                    new OperationParameter("format", "")
                }
            };
        }
    }
}