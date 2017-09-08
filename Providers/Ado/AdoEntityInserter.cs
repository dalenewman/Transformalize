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
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
    public class AdoEntityInserter : IWrite {
        private readonly OutputContext _output;
        private readonly IConnectionFactory _cf;

        public AdoEntityInserter(OutputContext output, IConnectionFactory cf) {
            _output = output;
            _cf = cf;
        }

        public void Write(IEnumerable<IRow> rows) {
            _output.Entity.InsertCommand = _output.SqlInsertIntoOutput(_cf);
            var count = (uint)0;
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var trans = cn.BeginTransaction();

                try {
                    foreach (var batch in rows.Partition(_output.Entity.InsertSize)) {
                        var records = batch.Select(r => r.ToExpandoObject(_output.OutputFields));
                        var batchCount = Convert.ToUInt32(cn.Execute(
                            _output.Entity.InsertCommand,
                            records,
                            trans,
                            0,
                            CommandType.Text
                        ));
                        count += batchCount;
                    }
                    trans.Commit();
                } catch (Exception ex) {
                    _output.Error(ex, ex.Message);
                    _output.Warn("Rolling back");
                    trans.Rollback();
                }
                _output.Debug(() => $"{count} to {_output.Connection.Name}");
            }
            _output.Entity.Inserts += count;
        }
    }
}