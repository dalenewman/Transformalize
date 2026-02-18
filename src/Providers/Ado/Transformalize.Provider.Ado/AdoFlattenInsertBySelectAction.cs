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

using System.Data;
using System.Data.Common;
using System.Text;
using Dapper;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
   public class AdoFlattenInsertBySelectAction : IAction {

      private readonly OutputContext _output;
      private readonly IConnectionFactory _cf;
      private readonly AdoSqlModel _model;
      private readonly IDbTransaction _trans;
      private readonly IDbConnection _cn;

      public AdoFlattenInsertBySelectAction(OutputContext output, IConnectionFactory cf, AdoSqlModel model, IDbConnection cn, IDbTransaction trans) {
         _output = output;
         _cf = cf;
         _model = model;
         _trans = trans;
         _cn = cn;
      }

      public ActionResponse Execute() {

         var masterAlias = Utility.GetExcelName(_model.MasterEntity.Index);
         var builder = new StringBuilder();
         builder.AppendLine($"INSERT INTO {_model.Flat}({string.Join(",", _model.Aliases)})");
         builder.AppendLine($"SELECT {_output.SqlStarFields(_cf)}");

         var close = _cf.AdoProvider != AdoProvider.Access ? string.Empty : ")";

         foreach (var from in _output.SqlStarFroms(_cf)) {
            builder.AppendLine(@from);
         }

         builder.AppendLine($"LEFT OUTER JOIN {_cf.Enclose(_output.Process.Name + _output.Process.FlatSuffix)} flat ON (flat.{_model.EnclosedKeyLongName} = {masterAlias}.{_model.EnclosedKeyShortName}){close}");
         builder.AppendLine($" WHERE flat.{_model.EnclosedKeyLongName} IS NULL AND {masterAlias}.{_model.Batch} > @Threshold; ");

         var command = builder.ToString();

         if (_cn.State != ConnectionState.Open) {
            _cn.Open();
         }

         try {
            _output.Debug(() => command);
            var count = _model.Threshold > 0 ? _cn.Execute(command, new { _model.Threshold }, commandTimeout: 0, transaction: _trans) : _cn.Execute(command, commandTimeout: 0, transaction: _trans);
            _output.Info($"{count} record{count.Plural()} inserted into flat");
         } catch (DbException ex) {
            return new ActionResponse(500, ex.Message);
         }

         return new ActionResponse(200, "Ok");
      }
   }
}