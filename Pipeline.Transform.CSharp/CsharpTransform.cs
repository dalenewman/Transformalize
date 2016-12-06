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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.CSharp {
    public class CsharpTransform : BaseTransform {
        private readonly CSharpHost.UserCodeInvoker _userCode;

        public CsharpTransform(IContext context) : base(context, null) {
            var name = Utility.GetMethodName(context);

            ConcurrentDictionary<string, CSharpHost.UserCodeInvoker> userCodes;
            if (CSharpHost.Cache.TryGetValue(context.Process.Name, out userCodes)) {
                if (userCodes.TryGetValue(name, out _userCode))
                    return;
            }

            context.Error($"Could not find {name} method in user's code");
            var dv = Constants.TypeDefaults()[context.Field.Type];
            _userCode = objects => dv;
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _userCode(row.ToArray());
            Increment();
            return row;
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            return rows.Select(Transform);
        }
    }
}