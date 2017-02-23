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
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dapper;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Provider.Ado.Ext;

namespace Transformalize.Provider.Ado {
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

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var trans = cn.BeginTransaction();
                try {

                    foreach (var batch in rows.Partition(_context.Entity.UpdateSize)) {
                        var data = batch.Select(r => r.ToExpandoObject(fields));
                        var batchCount = Convert.ToUInt32(cn.Execute(sql, data, trans, 0, CommandType.Text));
                        _context.Increment(batchCount);
                    }
                    trans.Commit();

                } catch (Exception ex) {
                    _context.Error(ex, ex.Message);
                    trans.Rollback();
                }
            }
        }
    }
}