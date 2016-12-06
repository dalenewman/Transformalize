#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Desktop.Transforms {
    public class CompiledRegexReplaceTransform : BaseTransform {
        private readonly Field _input;
        private readonly Regex _regex;
        private readonly Action<IRow> _transform;

        public CompiledRegexReplaceTransform(IContext context) : base(context, "string") {
            _input = SingleInput();
            _regex = new Regex(context.Transform.Pattern, RegexOptions.Compiled);
            if (context.Transform.Count == 0) {
                _transform = r => r[Context.Field] = _regex.Replace(r[_input].ToString(), context.Transform.NewValue);
            } else {
                _transform = r => r[Context.Field] = _regex.Replace(r[_input].ToString(), context.Transform.NewValue, context.Transform.Count);
            }
        }

        public override IRow Transform(IRow row) {
            _transform(row);
            Increment();
            return row;
        }
    }
}