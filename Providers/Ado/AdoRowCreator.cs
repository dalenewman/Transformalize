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

        private readonly IConnectionContext _context;
        private readonly IRowFactory _rowFactory;
        private bool[] _errors;
        private readonly Dictionary<string, Type> _typeMap;

        public AdoRowCreator(IConnectionContext context, IRowFactory rowFactory) {
            _errors = null;
            _context = context;
            _rowFactory = rowFactory;
            _typeMap = Constants.TypeSystem();
        }

        public IRow Create(IDataReader reader, Field[] fields) {

            var fieldCount = Math.Min(reader.FieldCount, fields.Length);
            var row = _rowFactory.Create();

            if (_errors == null) {  // check types
                _errors = new bool[fields.Length];
                for (var i = 0; i < fieldCount; i++) {
                    _errors[i] = reader.GetFieldType(i) != _typeMap[fields[i].Type];
                    if (!_errors[i])
                        continue;

                    if (fields[i].Type != "char") {
                        if (_context.Connection.Provider != "sqlite") {
                            _context.Warn("Type mismatch for {0}. Expected {1}, but read {2}.", fields[i].Name, fields[i].Type, reader.GetFieldType(i));
                        }
                    }
                }
            }

            for (var i = 0; i < fieldCount; i++) {
                if (reader.IsDBNull(i))
                    continue;
                if (_errors[i]) {
                    row[fields[i]] = fields[i].Convert(reader.GetValue(i));
                } else {
                    row[fields[i]] = reader.GetValue(i);
                }
            }
            return row;
        }

    }
}
