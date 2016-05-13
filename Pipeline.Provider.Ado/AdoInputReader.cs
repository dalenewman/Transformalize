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
using System.Data;
using System.Linq;
using Dapper;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Provider.Ado.Ext;
using Pipeline.Transforms.System;

namespace Pipeline.Provider.Ado {
    /// <summary>
    /// A reader for an entity's input (source).
    /// </summary>
    public class AdoInputReader : IRead {

        int _rowCount;
        readonly InputContext _input;
        readonly AdoRowCreator _rowCreator;
        readonly Field[] _fields;
        private readonly IConnectionFactory _factory;
        private readonly TypeTransform _typeTransform;

        public AdoInputReader(InputContext input, Field[] fields, IConnectionFactory factory, IRowFactory rowFactory) {
            _input = input;
            _fields = fields;
            _factory = factory;
            _rowCreator = new AdoRowCreator(input, rowFactory);
            _typeTransform = new TypeTransform(fields);
        }

        public IEnumerable<IRow> Read() {

            using (var cn = _factory.GetConnection()) {

                cn.Open();
                var cmd = cn.CreateCommand();

                if (string.IsNullOrEmpty(_input.Entity.Query)) {
                    if (_input.Entity.MinVersion == null) {
                        cmd.CommandText = _input.SqlSelectInput(_fields, _factory, _input.ResolveOrder);

                        if (_input.Entity.IsPageRequest()) {
                            var start = (_input.Entity.Page * _input.Entity.PageSize) - _input.Entity.PageSize;
                            var end = start + _input.Entity.PageSize;
                            switch (_factory.AdoProvider) {
                                case AdoProvider.SqlServer:
                                    cmd.CommandText = $"WITH p AS ({cmd.CommandText}) SELECT {string.Join(",", _fields.Select(f => _factory.Enclose(f.Name)))} FROM p WHERE TflRow BETWEEN {start + 1} AND {end}";
                                    break;
                                case AdoProvider.PostgreSql:
                                    cmd.CommandText += $" LIMIT {_input.Entity.PageSize} OFFSET {start}";
                                    break;
                                default:
                                    cmd.CommandText += $" LIMIT {start},{_input.Entity.PageSize}";
                                    break;
                            }
                        }

                        _input.Debug(() => cmd.CommandText);
                    } else {
                        cmd.CommandText = _input.SqlSelectInputWithMinVersion(_fields, _factory, _input.ResolveOrder);
                        _input.Debug(() => cmd.CommandText);

                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = "@MinVersion";
                        parameter.Direction = ParameterDirection.Input;
                        parameter.Value = _input.Entity.MinVersion;
                        cmd.Parameters.Add(parameter);
                    }

                    if (_input.Entity.IsPageRequest()) {
                        _input.Entity.Hits = cn.ExecuteScalar<int>($"SELECT COUNT(*) FROM {_input.SqlInputName(_factory)} {(_factory.AdoProvider == AdoProvider.SqlServer ? "WITH (NOLOCK)" : string.Empty)} {(_input.Entity.Filter.Any() ? _input.ResolveFilter(_factory) : string.Empty)}");
                    }

                } else {
                    cmd.CommandText = _input.Entity.Query;
                }

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess)) {

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