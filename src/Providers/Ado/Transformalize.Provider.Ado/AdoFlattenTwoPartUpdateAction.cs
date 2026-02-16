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
using System.Text;
using Dapper;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Ado {
   public class AdoFlattenTwoPartUpdateAction : IAction {

      private readonly OutputContext _output;
      private readonly IConnectionFactory _cf;
      private readonly AdoSqlModel _model;
      private readonly string _sql;
      private readonly IDbConnection _cn;
      private readonly IDbTransaction _trans;

      public AdoFlattenTwoPartUpdateAction(
          OutputContext output,
          IConnectionFactory cf,
          AdoSqlModel model,
          string sql,
          IDbConnection cn,
          IDbTransaction trans
      ) {
         _output = output;
         _cf = cf;
         _model = model;
         _sql = sql;
         _trans = trans;
         _cn = cn;
      }

      public ActionResponse Execute() {

         var updateVariables = string.Join(", ", _model.Fields.Where(f => f.Name != _model.KeyLongName).Select(f => $"{_cf.Enclose(f.Alias)} = @{f.FieldName()}"));

         var builder = new StringBuilder();
         builder.AppendLine($"UPDATE {_output.Process.Name + _output.Process.FlatSuffix}");
         builder.AppendLine($"SET {updateVariables}");
         builder.AppendLine($"WHERE {_model.EnclosedKeyLongName} = @{_model.KeyShortName};");

         var command = builder.ToString();


         var count = 0;

         try {
            foreach (var batch in Read(_cn, _sql, _model).Partition(_model.MasterEntity.UpdateSize)) {
               var expanded = batch.ToArray();
               _cn.Execute(command, expanded, _trans);
               count += expanded.Length;
            }
            _output.Info($"{count} record{count.Plural()} updated in flat");
         } catch (Exception ex) {
            _output.Error(ex.Message);
            _output.Error(_sql);
            return new ActionResponse(500, ex.Message) {
               Action = new Configuration.Action {
                  Type = "internal",
                  Description = "Flatten Action",
                  ErrorMode = "abort"
               }
            };
         }


         return new ActionResponse(200, "Ok");
      }

      private static IEnumerable<ExpandoObject> Read(IDbConnection cn, string sql, AdoSqlModel model) {

         using (var reader = cn.ExecuteReader(sql, new { model.Threshold }, null, 0, CommandType.Text)) {
            while (reader.Read()) {
               var obj = new ExpandoObject();
               var dict = (IDictionary<string, object>)obj;
               for (var i = 0; i < model.FieldNames.Length; i++) {
                  dict[model.FieldNames[i]] = reader.GetValue(i);
               }
               yield return obj;
            }
         }
      }
   }
}