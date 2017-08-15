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
using Transformalize.Contracts;
using Transformalize.Transforms;
using Transformalize.Configuration;

namespace Transformalize.Transform.DateMath {
    public class DateMathTransform : BaseTransform {
        private readonly Field _input;

        public DateMathTransform(IContext context) : base(context, "datetime") {
            if (IsNotReceiving("date")) {
                return;
            }

            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = DaleNewman.DateMath.Apply((DateTime)row[_input], Context.Transform.Expression);
            Increment();
            return row;
        }
    }
}
