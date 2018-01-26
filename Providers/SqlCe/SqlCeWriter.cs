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
using System.Data.SqlServerCe;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.SqlCe {

    public class SqlCeWriter : IWrite {
        private readonly OutputContext _output;
        private readonly IConnectionFactory _cf;
        private readonly IBatchReader _outputKeysReader;
        private readonly IWrite _sqlUpdater;

        public SqlCeWriter(
            OutputContext output,
            IConnectionFactory cf,
            IBatchReader matcher,
            IWrite updater
        ) {
            _output = output;
            _cf = cf;
            _outputKeysReader = matcher;
            _sqlUpdater = updater;
        }

        public void Write(IEnumerable<IRow> rows) {

            var table = _output.Entity.OutputTableName(_output.Process.Name);

            using (var cn = new SqlCeConnection(_cf.GetConnectionString())) {
                cn.Open();

                foreach (var part in rows.Partition(_output.Entity.InsertSize)) {

                    var batch = part.ToArray();

                    if (_output.Process.Mode == "init") {
                        var inserts = new List<IRow>();
                        inserts.AddRange(batch);
                        Insert(inserts, cn, table);
                    } else {
                        var inserts = new List<IRow>();
                        var updates = new List<IRow>();
                        var tflHashCode = _output.Entity.TflHashCode();
                        var tflDeleted = _output.Entity.TflDeleted();

                        var matching = _outputKeysReader.Read(batch);

                        for (int i = 0, batchLength = batch.Length; i < batchLength; i++) {
                            var row = batch[i];
                            if (matching.Contains(i)) {
                                if (matching[i][tflDeleted].Equals(true) || !matching[i][tflHashCode].Equals(row[tflHashCode])) {
                                    updates.Add(row);
                                }
                            } else {
                                inserts.Add(row);
                            }
                        }


                        Insert(inserts, cn, table);

                        if (updates.Any()) {_output.Entity.OutputTableName(_output.Process.Name);
                            _sqlUpdater.Write(updates);
                        }

                    }

                }

                if (_output.Entity.Inserts > 0) {
                    _output.Info("{0} inserts into {1} {2}", _output.Entity.Inserts, _output.Connection.Name, _output.Entity.Alias);
                }

                if (_output.Entity.Updates > 0) {
                    _output.Info("{0} updates to {1}", _output.Entity.Updates, _output.Connection.Name);
                }
            }

        }

        private void Insert(IEnumerable<IRow> inserts, SqlCeConnection cn, string table) {

            var enumerated = inserts.ToArray();

            if (enumerated.Length == 0)
                return;


            try {

                using (var cmd = new SqlCeCommand(table, cn, null)) {
                    cmd.CommandType = CommandType.TableDirect;
                    using (var rs = cmd.ExecuteResultSet(ResultSetOptions.Updatable)) {
                        foreach (var row in enumerated) {
                            var rec = rs.CreateRecord();
                            for (var i = 0; i < _output.OutputFields.Length; i++) {
                                var field = _output.OutputFields[i];
                                rec.SetValue(i, row[field]);
                            }
                            rs.Insert(rec);
                        }
                    }
                }

                _output.Entity.Inserts += Convert.ToUInt32(enumerated.Length);
            } catch (Exception ex) {
                _output.Error(ex.Message);
            }
        }

    }


}