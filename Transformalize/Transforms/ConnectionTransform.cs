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
using System.Reflection;
using Cfg.Net;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class ConnectionTransform : BaseTransform {

        private readonly object _value;

        public ConnectionTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.Name)) {
                return;
            }

            if (IsMissing(Context.Operation.Property)) {
                return;
            }

            string[] props;

#if NETS10
            props = typeof(Connection).GetRuntimeProperties().Where(prop => prop.GetCustomAttribute(typeof(CfgAttribute), true) != null).Select(prop => prop.Name).ToArray();
#else
            props = typeof(Connection).GetProperties().Where(prop => prop.GetCustomAttributes(typeof(CfgAttribute), true).FirstOrDefault() != null).Select(prop => prop.Name).ToArray();
#endif

            var set = new HashSet<string>(props, StringComparer.OrdinalIgnoreCase);

            if (!set.Contains(Context.Operation.Property)) {
                Error($"The connection property {Context.Operation.Property} is not allowed.  The allowed properties are {(string.Join(", ", props))}.");
                Run = false;
                return;
            }

            Context.Operation.Property = set.First(s => s.Equals(Context.Operation.Property, StringComparison.OrdinalIgnoreCase));

            var connection = Context.Process.Connections.First(c => c.Name.Equals(Context.Operation.Name, StringComparison.OrdinalIgnoreCase));
            _value = Utility.GetPropValue(connection, Context.Operation.Property);
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _value;

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("connection") {
                Parameters = new List<OperationParameter>(2){
                    new OperationParameter("name"),
                    new OperationParameter("property")
                }
            };
        }
    }
}