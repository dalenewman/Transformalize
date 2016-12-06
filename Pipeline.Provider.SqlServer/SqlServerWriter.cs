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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Provider.Ado;
using Transformalize.Provider.Ado.Ext;

namespace Transformalize.Provider.SqlServer {

    public class SqlServerWriter : IWrite {

        SqlBulkCopyOptions _bulkCopyOptions;
        readonly OutputContext _output;
        private readonly IConnectionFactory _cf;
        readonly ITakeAndReturnRows _outputKeysReader;
        readonly IWrite _sqlUpdater;
        readonly Field[] _keys;

        public SqlServerWriter(
            OutputContext output,
            IConnectionFactory cf,
            ITakeAndReturnRows matcher,
            IWrite updater
        ) {
            _output = output;
            _cf = cf;
            _bulkCopyOptions = SqlBulkCopyOptions.Default;

            TurnOptionOn(SqlBulkCopyOptions.TableLock);
            TurnOptionOn(SqlBulkCopyOptions.UseInternalTransaction);
            TurnOptionOff(SqlBulkCopyOptions.CheckConstraints);
            TurnOptionOff(SqlBulkCopyOptions.FireTriggers);
            TurnOptionOn(SqlBulkCopyOptions.KeepNulls);

            _keys = output.Entity.GetPrimaryKey();
            _outputKeysReader = matcher;
            _sqlUpdater = updater;
        }

        void TurnOptionOn(SqlBulkCopyOptions option) {
            _bulkCopyOptions |= option;
        }
        bool IsOptionOn(SqlBulkCopyOptions option) {
            return (_bulkCopyOptions & option) == option;
        }

        void TurnOptionOff(SqlBulkCopyOptions option) {
            if (IsOptionOn(option))
                _bulkCopyOptions ^= option;
        }

        public void Write(IEnumerable<IRow> rows) {

            using (var cn = new SqlConnection(_cf.GetConnectionString())) {
                cn.Open();

                SqlDataAdapter adapter;
                var dt = new DataTable();
                using (adapter = new SqlDataAdapter(_output.SqlSelectOutputSchema(_cf), cn)) {
                    adapter.Fill(dt);
                }

                var bulkCopy = new SqlBulkCopy(cn, _bulkCopyOptions, null) {
                    BatchSize = _output.Entity.InsertSize,
                    BulkCopyTimeout = 0,
                    DestinationTableName = "[" + _output.Entity.OutputTableName(_output.Process.Name) + "]"
                };

                for (var i = 0; i < _output.OutputFields.Length; i++) {
                    bulkCopy.ColumnMappings.Add(i, i);
                }

                var tflHashCode = _output.Entity.TflHashCode();
                var tflDeleted = _output.Entity.TflDeleted();

                foreach (var part in rows.Partition(_output.Entity.InsertSize)) {

                    var inserts = new List<DataRow>(_output.Entity.InsertSize);
                    var updates = new List<IRow>();
                    var batchCount = 0;

                    if (_output.Entity.IsFirstRun) {
                        foreach (var row in part) {
                            inserts.Add(GetDataRow(dt, row));
                            batchCount++;
                        }
                    } else {
                        var batch = part.ToArray();
                        var matching = _outputKeysReader.Read(batch).ToArray();

                        for (int i = 0, batchLength = batch.Length; i < batchLength; i++) {
                            var row = batch[i];
                            var match = matching.FirstOrDefault(f => f.Match(_keys, row));
                            if (match == null) {
                                inserts.Add(GetDataRow(dt, row));
                            } else {
                                if (match[tflDeleted].Equals(true)) {
                                    updates.Add(row);
                                } else {
                                    var destination = (int)match[tflHashCode];
                                    var source = (int)row[tflHashCode];
                                    if (source != destination) {
                                        updates.Add(row);
                                    }
                                }
                            }
                            batchCount++;
                        }
                    }

                    if (inserts.Any()) {
                        try {
                            bulkCopy.WriteToServer(inserts.ToArray());
                            _output.Entity.Inserts += inserts.Count;
                        } catch (Exception ex) {
                            _output.Error(ex.Message);
                        }
                    }

                    if (updates.Any()) {
                        _sqlUpdater.Write(updates);
                    }

                    _output.Increment(batchCount);
                }

                if (_output.Entity.Inserts > 0) {
                    _output.Info("{0} inserts into {1} {2}", _output.Entity.Inserts, _output.Connection.Name, _output.Entity.Alias);
                }

                if (_output.Entity.Updates > 0) {
                    _output.Info("{0} updates to {1}", _output.Entity.Updates, _output.Connection.Name);
                }
            }

        }

        DataRow GetDataRow(DataTable dataTable, IRow row) {
            var dr = dataTable.NewRow();
            dr.ItemArray = row.ToEnumerable(_output.OutputFields).ToArray();
            return dr;
        }

    }
}