#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using System.Linq;
using Dapper;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.Ado {

    public class AdoMasterUpdater : IUpdate {
        readonly Entity _master;
        readonly OutputContext _output;
        private readonly IConnectionFactory _cf;
        private readonly IWriteMasterUpdateQuery _queryWriter;

        public AdoMasterUpdater(OutputContext output, IConnectionFactory cf, IWriteMasterUpdateQuery queryWriter) {
            _output = output;
            _cf = cf;
            _queryWriter = queryWriter;
            _master = _output.Process.Entities.First(e => e.IsMaster);
        }

        public void Update()
        {
            var status = _output.GetEntityStatus();
            if (!status.NeedsUpdate())
                return;

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var sql = _queryWriter.Write(status);
                try {
                    var rowCount = cn.Execute(sql, new {
                        TflBatchId = _output.Entity.BatchId,
                        MasterTflBatchId = _master.BatchId
                    }, null, 0, System.Data.CommandType.Text);
                    _output.Info(rowCount + " updates to master");
                } catch (Exception ex) {
                    _output.Error("error executing: {0}", sql);
                    _output.Error(ex, ex.Message);
                }
            }
        }
    }
}
