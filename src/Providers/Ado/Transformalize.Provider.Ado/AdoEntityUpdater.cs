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
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Transforms.System;

namespace Transformalize.Providers.Ado {
   public class AdoEntityUpdater : IWrite {

      private readonly OutputContext _output;
      private readonly IConnectionFactory _cf;
      private readonly IOperation _minDates;

      public AdoEntityUpdater(OutputContext output, IConnectionFactory cf) {
         _output = output;
         _cf = cf;

         if (output.Connection.Provider == "sqlserver" && output.OutputFields.Any(f => f.Type.StartsWith("date"))) {
            _minDates = new MinDateTransform(output, new DateTime(1753, 1, 1));
         }
      }

      public void Write(IEnumerable<IRow> rows) {
         InternalWrite(_minDates != null ? _minDates.Operate(rows) : rows);
      }

      private void InternalWrite(IEnumerable<IRow> rows) {
         _output.Entity.UpdateCommand = _output.SqlUpdateOutput(_cf);
         var count = (uint)0;
         using (var cn = _cf.GetConnection()) {
            cn.Open();
            _output.Debug(() => "begin transaction");
            var trans = cn.BeginTransaction();
            try {
               foreach (var batch in rows.Partition(_output.Entity.UpdateSize)) {
                  var batchCount = Convert.ToUInt32(cn.Execute(
                      _output.Entity.UpdateCommand,
                      batch.Select(r => r.ToExpandoObject(_output.GetUpdateFields().ToArray())),
                      trans,
                      0,
                      CommandType.Text
                  ));
                  count += batchCount;
               }
               _output.Debug(() => "commit transaction");
               trans.Commit();
               _output.Entity.Updates += count;
               _output.Info("{0} to {1}", count, _output.Connection.Name);
            } catch (Exception ex) {
               _output.Error(ex.Message);
               _output.Warn("rollback transaction");
               trans.Rollback();
            }
         }

      }

   }
}