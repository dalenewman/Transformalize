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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {

    public class AdoRowCreator {

        private readonly IConnectionContext _context;
        private readonly IRowFactory _rowFactory;
        private bool[] _errors;
        private readonly Dictionary<string, Type> _typeMap;
        private readonly List<Func<object, object>> _conversions = new List<Func<object, object>>();

        public AdoRowCreator(IConnectionContext context, IRowFactory rowFactory) {
            _errors = null;
            _context = context;
            _rowFactory = rowFactory;
            _typeMap = Constants.TypeSystem();
        }

        public IRow Create(IDataReader reader, Field[] fields) {

            var fieldCount = Math.Min(reader.FieldCount, fields.Length);
            var row = _rowFactory.Create();
            for (var i = 0; i < fieldCount; i++) {
                _conversions.Add(null);
            }

            if (_errors == null) {  // check types
                _errors = new bool[fields.Length];
                for (var i = 0; i < fieldCount; i++) {
                    _errors[i] = reader.GetFieldType(i) != _typeMap[fields[i].Type];

                    if (_errors[i]) {
                        if (fields[i].Transforms.Any() && fields[i].Transforms.First().Method == "convert") {
                            _conversions[i] = o => o;  // the user has set a conversion
                        } else {
                            _conversions[i] = fields[i].Convert;
                        }
                        if (fields[i].Type != "char") {
                            if (_context.Connection.Provider != "sqlite") {
                                _context.Warn("Type mismatch for {0}. Expected {1}, but read {2}.", fields[i].Name, fields[i].Type, reader.GetFieldType(i));
                            }
                        }
                    } else {
                        _conversions[i] = o => o;
                    }

                }
                for (var i = 0; i < fieldCount; i++) {
                    if (reader.IsDBNull(i))
                        continue;
                    if (_errors[i]) {
                        var value = reader.GetValue(i);
                        try {
                            row[fields[i]] = _conversions[i](value);
                        } catch (FormatException) {
                            _context.Error($"Could not convert value {value} in field {fields[i].Alias} to {fields[i].Type}");
                        }

                    } else {
                        row[fields[i]] = reader.GetValue(i);
                    }
                }
            } else {
                for (var i = 0; i < fieldCount; i++) {
                    if (reader.IsDBNull(i))
                        continue;
                    if (_errors[i]) {
                        row[fields[i]] = _conversions[i](reader.GetValue(i));
                    } else {
                        row[fields[i]] = reader.GetValue(i);
                    }
                }
            }


            return row;
        }

    }
}
