#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Extensions;
using Pipeline.Provider.Ado.Ext;

namespace Pipeline.Provider.Ado {
    public class AdoEntityInserter : IWrite {
        readonly OutputContext _output;
        private readonly IConnectionFactory _cf;

        public AdoEntityInserter(OutputContext output, IConnectionFactory cf) {
            _output = output;
            _cf = cf;
        }

        public void Write(IEnumerable<IRow> rows) {
            var sql = _output.SqlInsertIntoOutput(_cf);
            var count = 0;
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var trans = cn.BeginTransaction();

                try {
                    foreach (var batch in rows.Partition(_output.Entity.InsertSize)) {
                        var batchCount = cn.Execute(
                            sql,
                            batch.Select(r => r.ToExpandoObject(_output.OutputFields)),
                            trans,
                            0,
                            CommandType.Text
                        );
                        count += batchCount;
                        _output.Increment(batchCount);
                    }
                    trans.Commit();
                } catch (Exception ex) {
                    _output.Error(ex, ex.Message);
                    trans.Rollback();
                }
                _output.Debug(() => $"{count} to {_output.Connection.Name}");
            }
            _output.Entity.Inserts += count;
        }
    }
}