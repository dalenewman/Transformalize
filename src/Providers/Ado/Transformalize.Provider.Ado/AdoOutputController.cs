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
using Dapper;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {

    public class AdoOutputController : BaseOutputController {

        private readonly IConnectionFactory _cf;

        private readonly Stopwatch _stopWatch;

        public AdoOutputController(
            OutputContext context,
            IAction initializer,
            IInputProvider inputProvider,
            IOutputProvider outputProvider,
            IConnectionFactory cf) : base(context, initializer, inputProvider, outputProvider) {
            _cf = cf;
            _stopWatch = new Stopwatch();
        }

        public override void Start() {

            if (_cf.AdoProvider == AdoProvider.Access) {
                SqlMapper.AddTypeMap(typeof(DateTime), DbType.Date);
            }

            _stopWatch.Start();
            base.Start();

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                Context.Debug(() => "Loading BatchId.");
                var sql = Context.SqlControlStartBatch(_cf);
                try {
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    var batchId = cmd.CreateParameter();
                    batchId.ParameterName = "BatchId";
                    batchId.DbType = DbType.Int32;
                    batchId.Value = Context.Entity.BatchId;

                    var entity = cmd.CreateParameter();
                    entity.ParameterName = "Entity";
                    entity.DbType = DbType.String;
                    entity.Value = Context.Entity.Alias;

                    var mode = cmd.CreateParameter();
                    mode.ParameterName = "Mode";
                    mode.DbType = DbType.String;
                    mode.Value = Context.Process.Mode;

                    var start = cmd.CreateParameter();
                    start.ParameterName = "Start";
                    start.DbType = _cf.AdoProvider == AdoProvider.Access ? DbType.Date : DbType.DateTime;
                    start.Value = DateTime.Now;

                    cmd.Parameters.Add(batchId);
                    cmd.Parameters.Add(entity);
                    cmd.Parameters.Add(mode);
                    cmd.Parameters.Add(start);

                    cmd.ExecuteNonQuery();

                } catch (Exception e) {
                    Context.Error(e.Message);
                }
                if (cn.State != ConnectionState.Closed) {
                    cn.Close();
                }
            }

        }

        public override void End() {
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var sql = Context.SqlControlEndBatch(_cf);
                if (_cf.AdoProvider == AdoProvider.Access) {
                    var cmd = cn.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    var inserts = cmd.CreateParameter();
                    inserts.ParameterName = "Inserts";
                    inserts.DbType = DbType.Int32;
                    inserts.Value = Convert.ToInt32(Context.Entity.Inserts);

                    var updates = cmd.CreateParameter();
                    updates.ParameterName = "Updates";
                    updates.DbType = DbType.Int32;
                    updates.Value = Convert.ToInt32(Context.Entity.Updates);

                    var deletes = cmd.CreateParameter();
                    deletes.ParameterName = "Deletes";
                    deletes.DbType = DbType.Int32;
                    deletes.Value = Convert.ToInt32(Context.Entity.Deletes);

                    var end = cmd.CreateParameter();
                    end.ParameterName = "End";
                    end.DbType = DbType.Date;
                    end.Value = DateTime.Now;

                    var entity = cmd.CreateParameter();
                    entity.ParameterName = "Entity";
                    entity.DbType = DbType.String;
                    entity.Value = Context.Entity.Alias;

                    var batchId = cmd.CreateParameter();
                    batchId.ParameterName = "BatchId";
                    batchId.DbType = DbType.Int32;
                    batchId.Value = Context.Entity.BatchId;

                    cmd.Parameters.Add(inserts);
                    cmd.Parameters.Add(updates);
                    cmd.Parameters.Add(deletes);
                    cmd.Parameters.Add(end);
                    cmd.Parameters.Add(entity);
                    cmd.Parameters.Add(batchId);

                    cmd.ExecuteNonQuery();

                } else {
                    try {
                        cn.Execute(sql, new {
                            Inserts = Convert.ToInt64(Context.Entity.Inserts),
                            Updates = Convert.ToInt64(Context.Entity.Updates),
                            Deletes = Convert.ToInt64(Context.Entity.Deletes),
                            End = DateTime.Now,
                            Entity = Context.Entity.Alias,
                            Input = Context.Entity.Input,
                            Context.Entity.BatchId
                        });
                    } catch (System.Data.Common.DbException e) {
                        Context.Error("Error writing to the control table.");
                        Context.Error(e.Message);
                        Context.Debug(() => e.StackTrace);
                        Context.Debug(() => sql);
                        return;
                    }

                }
                if (cn.State != ConnectionState.Closed) {
                    cn.Close();
                }
            }

            OutputProvider.End();

            _stopWatch.Stop();
            Context.Debug(() => $"Entity {Context.Entity} ending {_stopWatch.Elapsed}");
        }

    }
}