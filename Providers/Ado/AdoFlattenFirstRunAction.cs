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

using System.Data.Common;
using Dapper;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
    public class AdoFlattenFirstRunAction : IAction {

        private readonly OutputContext _context;
        private readonly IConnectionFactory _cf;
        private readonly AdoSqlModel _model;

        public AdoFlattenFirstRunAction(OutputContext context, IConnectionFactory cf, AdoSqlModel model) {
            _context = context;
            _cf = cf;
            _model = model;
        }

        public ActionResponse Execute() {
            var message = "Ok";

            var sqlInsert = _cf.AdoProvider == AdoProvider.SqlCe ?
                $"INSERT INTO {_model.Flat}({string.Join(",", _model.Aliases)}) {_context.SqlSelectStar(_cf)}{_cf.Terminator}" :
                $"INSERT INTO {_model.Flat}({string.Join(",", _model.Aliases)}) SELECT {string.Join(",", _model.Aliases)} FROM {_model.Star}{_cf.Terminator}";

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var trans = cn.BeginTransaction();

                try {
                    _context.Debug(() => sqlInsert);
                    var count = cn.Execute(sqlInsert, commandTimeout: 0, transaction: trans);
                    message = $"{count} record{count.Plural()} inserted into flat";
                    _context.Info(message);
                    trans.Commit();
                } catch (DbException ex) {
                    trans.Rollback();
                    return new ActionResponse(500, ex.Message) {
                        Action = new Action {
                            Type = "internal",
                            Description = "Flatten Action",
                            ErrorMode = "abort"
                        }
                    };
                }

            }

            return new ActionResponse(200, message);

        }
    }
}