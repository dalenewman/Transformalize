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
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Extensions;
using Pipeline.Provider.Ado.Ext;

namespace Pipeline.Provider.Ado {
    public class AdoCalculatedFieldUpdater : IWrite {
        private readonly OutputContext _context;
        private readonly Process _parent;
        private readonly IConnectionFactory _cf;

        public AdoCalculatedFieldUpdater(OutputContext context, Process parent, IConnectionFactory cf) {
            _context = context;
            _parent = parent;
            _cf = cf;
        }

        public void Write(IEnumerable<IRow> rows) {
            var sql = _context.SqlUpdateCalculatedFields(_parent, _cf);
            var temp = new List<Field> { _context.Entity.TflKey() };
            temp.AddRange(_context.Entity.CalculatedFields.Where(f => f.Output));
            var fields = temp.ToArray();

            var count = 0;
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var trans = cn.BeginTransaction();
                try {
                    foreach (var batch in rows.Partition(_context.Entity.UpdateSize)) {
                        var batchCount = cn.Execute(
                            sql,
                            batch.Select(r => r.ToExpandoObject(fields)),
                            trans,
                            0,
                            CommandType.Text
                        );
                        count += batchCount;
                        _context.Increment(batchCount);
                    }
                    trans.Commit();

                } catch (Exception ex) {
                    _context.Error(ex, ex.Message);
                    trans.Rollback();
                }
            }
            //_context.Entity.Updates += count;
        }
    }
}