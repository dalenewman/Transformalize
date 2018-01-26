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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class DistinctTransform : BaseTransform {
        private readonly Field _input;
        private readonly char[] _sep;

        public DistinctTransform(IContext context) : base(context, "string") {
            _input = SingleInput();

            // check input type
            var typeReceived = Received();
            if (typeReceived != "string") {
                Error($"The distinct transform takes a string input.  You have a {typeReceived} input.");
            }

            // check separator
            if (context.Operation.Separator == Constants.DefaultSetting) {
                context.Operation.Separator = " ";
            }

            _sep = context.Operation.Separator.ToCharArray();

        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = string.Join(Context.Operation.Separator, ((string)row[_input]).Split(_sep, StringSplitOptions.RemoveEmptyEntries).Distinct());
            
            return row;
        }

    }
}