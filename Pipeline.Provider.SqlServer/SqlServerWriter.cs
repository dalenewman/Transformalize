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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

                foreach (var part in rows.Partition(_output.Entity.InsertSize)) {

                    var batch = part.ToArray();

                    if (_output.Entity.IsFirstRun) {
                        var inserts = new List<IRow>();
                        inserts.AddRange(batch);
                        Insert(bulkCopy, dt, inserts);
                    } else {
                        var inserts = new ConcurrentBag<IRow>();
                        var updates = new ConcurrentBag<IRow>();
                        var tflHashCode = _output.Entity.TflHashCode();
                        var tflDeleted = _output.Entity.TflDeleted();
                        var matching = _outputKeysReader.Read(batch).AsParallel().ToArray();

                        Parallel.ForEach(batch, (row) => {
                            var match = matching.FirstOrDefault(f => f.Match(_keys, row));
                            if (match == null) {
                                inserts.Add(row);
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
                        });

                        Insert(bulkCopy, dt, inserts);

                        if (updates.Any()) {
                            _sqlUpdater.Write(updates);
                        }

                    }

                    _output.Increment(batch.Length);
                }

                if (_output.Entity.Inserts > 0) {
                    _output.Info("{0} inserts into {1} {2}", _output.Entity.Inserts, _output.Connection.Name, _output.Entity.Alias);
                }

                if (_output.Entity.Updates > 0) {
                    _output.Info("{0} updates to {1}", _output.Entity.Updates, _output.Connection.Name);
                }
            }

        }

        private void Insert(SqlBulkCopy bulkCopy, DataTable dt, IEnumerable<IRow> inserts) {

            var enumerated = inserts.ToArray();

            if (enumerated.Length == 0)
                return;

            // convert to data rows for bulk copy
            var rows = new List<DataRow>();
            foreach (var insert in enumerated) {
                var row = dt.NewRow();
                row.ItemArray = insert.ToEnumerable(_output.OutputFields).ToArray();
                rows.Add(row);
            }

            try {
                bulkCopy.WriteToServer(rows.ToArray());
                _output.Entity.Inserts += enumerated.Length;
            } catch (Exception ex) {
                _output.Error(ex.Message);
            }
        }

    }


}