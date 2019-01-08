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
using System.Linq;
using Newtonsoft.Json;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Ioc.Autofac {
    public class JsonNetSerializer : ISerialize {
        private readonly Field[] _fields;

        public JsonNetSerializer(OutputContext context) {
            _fields = context.OutputFields.Where(f => !f.System).ToArray();
        }

        public string Serialize(IRow row) {
            return JsonConvert.SerializeObject(row.ToFriendlyExpandoObject(_fields));
        }

        string ISerialize.Header { get; } = "[";
        string ISerialize.Footer { get; } = "]";
        string ISerialize.RowSuffix { get; } = ",";
        public string RowPrefix { get; } = "   ";

    }
}