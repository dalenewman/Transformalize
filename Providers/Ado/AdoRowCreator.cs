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
using System.Data;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {
    public class AdoRowCreator {
        readonly IContext _context;
        private readonly IRowFactory _rowFactory;
        readonly HashSet<int> _errors;

        public AdoRowCreator(IContext context, IRowFactory rowFactory) {
            _errors = new HashSet<int>();
            _context = context;
            _rowFactory = rowFactory;
        }

        public IRow Create(IDataReader reader, Field[] fields) {

            var fieldCount = Math.Min(reader.FieldCount, fields.Length);
            var row = _rowFactory.Create();
            for (var i = 0; i < fieldCount; i++) {
                var field = fields[i];
                if (field.Type == "string") {
                    if (reader.GetFieldType(i) == typeof(string)) {
                        row[field] = reader.IsDBNull(i) ? null : reader.GetString(i);
                    } else {
                        TypeMismatch(field, reader, i);
                        var value = reader.GetValue(i);
                        row[field] = value == DBNull.Value ? null : value;
                    }
                } else {
                    var value = reader.GetValue(i);
                    row[field] = value == DBNull.Value ? null : value;
                }
            }
            return row;
        }

        public void TypeMismatch(Field field, IDataReader reader, int index) {
            var key = GetHashCode(field.Name, field.Type);
            if (_errors.Add(key)) {
                _context.Error("Type mismatch for {0}. Expected {1}, but read {2}.", field.Name, field.Type, reader.GetFieldType(index));
            }
        }

        static int GetHashCode<T>(T a, T b) {
            unchecked {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ a.GetHashCode();
                hash = hash * 16777619 ^ b.GetHashCode();
                return hash;
            }
        }

    }
}
