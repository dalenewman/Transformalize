#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Validators {
    public class EqualsValidator : BaseTransform, ITransform {
        private readonly Field _first;
        private readonly Field[] _rest;
        private readonly object _value;
        private readonly Action<IRow> _validator;

        public EqualsValidator(IContext context) : base(context) {
            bool sameTypes;
            var input = MultipleInput();
            _first = input.First();

            if (context.Transform.Value == Constants.DefaultSetting) {
                _rest = input.Skip(1).ToArray();
                sameTypes = _rest.All(f => f.Type == _first.Type);
            } else {
                _value = _first.Convert(context.Transform.Value);
                _rest = input.ToArray();
                sameTypes = input.All(f => f.Type == _first.Type);
            }

            if (sameTypes) {
                if (_value == null) {
                    _validator = row => row[Context.Field] = _rest.All(f => row[f].Equals(row[_first]));
                } else {
                    _validator = row => row[Context.Field] = _rest.All(f => row[f].Equals(_value));
                }
            } else {
                _validator = row => row[Context.Field] = false;
            }
        }

        public override IRow Transform(IRow row) {
            _validator(row);
            Increment();
            return row;
        }
    }
}