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
using System.Linq;
using Dapper;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Ado {

   /// <summary>
   /// todo: move the different query writing to each provider
   /// </summary>
   public class AdoFlattenUpdateByViewAction : IAction {

      private readonly OutputContext _output;
      private readonly AdoSqlModel _model;
      private readonly IDbTransaction _trans;
      private readonly IDbConnection _cn;

      public AdoFlattenUpdateByViewAction(OutputContext output, AdoSqlModel model, IDbConnection cn, IDbTransaction trans) {
         _output = output;
         _model = model;
         _cn = cn;
         _trans = trans;
      }

      public ActionResponse Execute() {

         var prefix = _model.AdoProvider == AdoProvider.PostgreSql ? string.Empty : "f.";
         var updates = string.Join(", ", _model.Aliases.Where(f => f != _model.EnclosedKeyLongName).Select(f => $"{prefix}{f} = s.{f}"));
         string command;

         if(_model.AdoProvider == AdoProvider.PostgreSql) {
            command = $@"
UPDATE {_model.Flat} f
SET {updates}
FROM {_model.Master} m, {_model.Star} s
WHERE m.{_model.EnclosedKeyShortName} = f.{_model.EnclosedKeyLongName}
AND f.{_model.EnclosedKeyLongName} = s.{_model.EnclosedKeyLongName}
AND m.{_model.Batch} > @Threshold;
";
         } else {
            command = $@"
UPDATE f
SET {updates}
FROM {_model.Flat} f
INNER JOIN {_model.Master} m ON (m.{_model.EnclosedKeyShortName} = f.{_model.EnclosedKeyLongName})
INNER JOIN {_model.Star} s ON (f.{_model.EnclosedKeyLongName} = s.{_model.EnclosedKeyLongName})
WHERE m.{_model.Batch} > @Threshold;
";
         }

         if(_cn.State != ConnectionState.Open) {
            _cn.Open();
         }

         try {
            _output.Debug(() => command);
            var count = _cn.Execute(command, new { _model.Threshold }, commandTimeout: 0, transaction: _trans);
            _output.Info($"{count} record{count.Plural()} updated in flat");
         } catch (DbException ex) {
            return new ActionResponse(500, ex.Message) { Action = new Action { Type = "internal", Description = "Flatten Action", ErrorMode = "abort" } };
         }

         return new ActionResponse(200, "Ok");
      }
   }
}