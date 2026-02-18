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

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Transforms.System;

namespace Transformalize.Providers.Ado {
   public class AdoCalculatedFieldUpdater : IWrite {
      private readonly OutputContext _context;
      private readonly Process _parent;
      private readonly IConnectionFactory _cf;
      private readonly IOperation _minDates;

      public AdoCalculatedFieldUpdater(OutputContext context, Process parent, IConnectionFactory cf) {
         _context = context;
         _parent = parent;
         _cf = cf;

         if (context.Connection.Provider == "sqlserver" && context.OutputFields.Any(f => f.Type.StartsWith("date"))) {
            _minDates = new MinDateTransform(context, new DateTime(1753, 1, 1));
         }
      }

      public void Write(IEnumerable<IRow> rows) {
         InternalWrite(_minDates != null ? _minDates.Operate(rows) : rows);
      }

      private void InternalWrite(IEnumerable<IRow> rows) {
         var sql = _context.SqlUpdateCalculatedFields(_parent, _cf);
         var fields = _context.GetUpdateCalculatedFields().ToArray();

         using (var cn = _cf.GetConnection()) {
            cn.Open();
            var trans = cn.BeginTransaction();
            try {

               foreach (var batch in rows.Partition(_context.Entity.UpdateSize)) {
                  _context.Debug(() => "got a batch!");
                  var data = batch.Select(r => r.ToExpandoObject(fields));
                  _context.Debug(() => "converted to expando object");
                  var batchCount = Convert.ToUInt32(cn.Execute(sql, data, trans, 0, CommandType.Text));
                  _context.Debug(() => $"Updated {batchCount} calculated field records!");
               }
               trans.Commit();
               _context.Debug(() => "Committed updates.");

            } catch (Exception ex) {
               _context.Error(ex, ex.Message);
               trans.Rollback();
            }
         }
      }


   }
}