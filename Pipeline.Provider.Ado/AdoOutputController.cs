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
using System.Data;
using System.Diagnostics;
using System.Net.Mime;
using Dapper;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado.Ext;

namespace Transformalize.Provider.Ado {
    public class AdoOutputController : BaseOutputController {
        private readonly IConnectionFactory _cf;

        readonly Stopwatch _stopWatch;

        public AdoOutputController(
            OutputContext context,
            IAction initializer,
            IInputProvider inputProvider,
            IOutputProvider outputProvider,
            IConnectionFactory cf) : base(context, initializer, inputProvider, outputProvider) {
            _cf = cf;
            _stopWatch = new Stopwatch();
        }

        int GetBatchId(IDbConnection cn) {
            return cn.ExecuteScalar<int>(Context.SqlControlLastBatchId(_cf)) + 1;
        }

        int GetIdentity(IDbConnection cn) {
            return cn.ExecuteScalar<int>($"SELECT MAX({_cf.Enclose(Context.Entity.TflKey().FieldName())}) FROM {_cf.Enclose(Context.Entity.OutputTableName(Context.Process.Name))};");
        }

        public override void Start() {
            _stopWatch.Start();
            base.Start();

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                Context.Debug(() => "Loading BatchId.");
                Context.Entity.BatchId = GetBatchId(cn);
                Context.Entity.Identity = GetIdentity(cn);
                var sql = Context.SqlControlStartBatch(_cf);
                cn.Execute(sql, new {
                    Context.Entity.BatchId,
                    Entity = Context.Entity.Alias,
                    DateTime.Now
                });

            }

        }

        public override void End() {
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var sql = Context.SqlControlEndBatch(_cf);
                cn.Execute(sql, new {
                    Inserts =  Convert.ToInt64(Context.Entity.Inserts),
                    Updates = Convert.ToInt64(Context.Entity.Updates),
                    Deletes = Convert.ToInt64(Context.Entity.Deletes),
                    Entity = Context.Entity.Alias,
                    Context.Entity.BatchId,
                    DateTime.Now
                });

            }
            _stopWatch.Stop();
            Context.Debug(()=>$"Entity {Context.Entity} ending {_stopWatch.Elapsed}");
        }

    }
}