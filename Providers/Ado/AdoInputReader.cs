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
using Dapper;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Transforms.System;

namespace Transformalize.Providers.Ado {
    /// <summary>
    /// A reader for an entity's input (source).
    /// </summary>
    public class AdoInputReader : IRead {

        private int _rowCount;
        private readonly InputContext _input;
        private readonly AdoRowCreator _rowCreator;
        private readonly Field[] _fields;
        private readonly IConnectionFactory _factory;
        private readonly TypeTransform _typeTransform;

        public AdoInputReader(InputContext input, Field[] fields, IConnectionFactory factory, IRowFactory rowFactory) {
            _input = input;
            _fields = fields;
            _factory = factory;
            _rowCreator = new AdoRowCreator(input, rowFactory);
            _typeTransform = new TypeTransform(input, fields);
        }

        public IEnumerable<IRow> Read() {

            using (var cn = _factory.GetConnection()) {

                cn.Open();
                var cmd = cn.CreateCommand();

                if (string.IsNullOrEmpty(_input.Entity.Query)) {
                    if (_input.Entity.MinVersion == null) {
                        cmd.CommandText = _input.SqlSelectInput(_fields, _factory);
                        _input.Debug(() => cmd.CommandText);
                    } else {
                        cmd.CommandText = _input.SqlSelectInputWithMinVersion(_fields, _factory);
                        _input.Debug(() => cmd.CommandText);

                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = "@MinVersion";
                        parameter.Direction = ParameterDirection.Input;
                        parameter.Value = _input.Entity.MinVersion;
                        cmd.Parameters.Add(parameter);
                    }

                    if (_input.Entity.IsPageRequest()) {
                        var sql = $"SELECT COUNT(*) FROM {_input.SqlInputName(_factory)} {(_factory.AdoProvider == AdoProvider.SqlServer ? "WITH (NOLOCK)" : string.Empty)} {(_input.Entity.Filter.Any() ? " WHERE " + _input.ResolveFilter(_factory) : string.Empty)}";
                        _input.Debug(() => sql);
                        try {
                            _input.Entity.Hits = cn.ExecuteScalar<int>(sql);
                        } catch (Exception ex) {
                            _input.Error(ex.Message);
                        }
                    }
                    _input.Entity.Query = cmd.CommandText;
                } else {
                    cmd.CommandText = _input.Entity.Query;
                }

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                IDataReader reader = null;
                try {
                    reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                } catch (Exception ex) {
                    _input.Error(ex.Message);
                    yield break;
                }

                using (reader) {

                    if (_fields.Length < reader.FieldCount) {
                        _input.Warn($"The reader is returning {reader.FieldCount} fields, but the entity {_input.Entity.Alias} expects {_fields.Length}!");
                    }

                    // transform types if sqlite
                    if (_factory.AdoProvider == AdoProvider.SqLite) {
                        while (reader.Read()) {
                            _rowCount++;
                            _input.Increment();
                            var row = _rowCreator.Create(reader, _fields);
                            _typeTransform.Transform(row);
                            yield return row;
                        }
                    } else {
                        // check the first one's types
                        if (reader.Read()) {
                            var row = _rowCreator.Create(reader, _fields);
                            foreach (var field in _fields) {
                                var expected = Constants.TypeSystem()[field.Type];
                                var actual = row[field] == null ? expected : row[field].GetType();
                                if (expected != actual) {
                                    _input.Warn($"The {field.Alias} field in {_input.Entity.Alias} expects a {expected}, but is reading a {actual}.");
                                }
                            }
                            _rowCount++;
                            _input.Increment();
                            yield return row;
                        }

                        // just read
                        while (reader.Read()) {
                            _rowCount++;
                            _input.Increment();
                            yield return _rowCreator.Create(reader, _fields);
                        }

                    }
                }

                _input.Info("{0} from {1}", _rowCount, _input.Connection.Name);
            }
        }

    }
}