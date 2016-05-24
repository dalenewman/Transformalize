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
using System.Collections.Generic;
using System.Linq;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline {
    public class InternalReader : IRead {

        readonly InputContext _input;
        private readonly IRowFactory _rowFactory;
        private readonly HashSet<string> _missing;

        public InternalReader(InputContext input, IRowFactory rowFactory) {
            _input = input;
            _rowFactory = rowFactory;
            _missing = new HashSet<string>();
        }

        public IEnumerable<IRow> Read() {
            var fields = _input.Entity.Fields.Where(f => f.Input).ToArray();
            var rows = new List<IRow>();
            foreach (var row in _input.Entity.Rows) {

                var typed = _rowFactory.Create();
                foreach (var field in fields) {
                    if (row.Map.ContainsKey(field.Name)) {
                        typed[field] = field.Convert(row[field.Name]);
                    } else {
                        if (_missing.Add(field.Name)) {
                            _input.Warn($"An internal row in {_input.Entity.Alias} is missing the field {field.Name}.");
                        }
                    }
                }
                rows.Add(typed);
            }
            return rows;
        }

        public object GetVersion() {
            return null;
        }
    }
}