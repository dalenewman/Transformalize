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

using System.Collections.Generic;
using System.Data;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Ado {
    public class AdoStarParametersReader : IRead {
        private readonly OutputContext _output;
        private readonly Process _parent;
        private readonly IConnectionFactory _cf;
        private readonly AdoRowCreator _rowCreator;

        public AdoStarParametersReader(OutputContext output, Process parent, IConnectionFactory cf, IRowFactory rowFactory) {
            _output = output;
            _parent = parent;
            _cf = cf;
            _rowCreator = new AdoRowCreator(output, rowFactory);
        }

        public IEnumerable<IRow> Read() {

            var batches = _parent.Entities.Select(e => e.BatchId).ToArray();
            var minBatchId = batches.Min();
            var maxBatchId = batches.Max();
            _output.Info("Batch Range: {0} to {1}.", minBatchId, maxBatchId);

            var sql = $"SELECT {string.Join(",", _output.Entity.Fields.Where(f=>f.Output).Select(f => _cf.Enclose(f.Alias)))} FROM {_cf.Enclose(_output.Process.Star)} {(_cf.AdoProvider == AdoProvider.SqlServer ? "WITH (NOLOCK)" : string.Empty)} WHERE {_cf.Enclose(Constants.TflBatchId)} BETWEEN @MinBatchId AND @MaxBatchId;";
            _output.Debug(() => sql);

            using (var cn = _cf.GetConnection()) {
                cn.Open();

                var cmd = cn.CreateCommand();

                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                var min = cmd.CreateParameter();
                min.ParameterName = "@MinBatchId";
                min.Value = minBatchId;
                min.Direction = ParameterDirection.Input;
                min.DbType = DbType.Int32;

                var max = cmd.CreateParameter();
                max.ParameterName = "@MaxBatchId";
                max.Value = maxBatchId;
                max.Direction = ParameterDirection.Input;
                max.DbType = DbType.Int32;

                cmd.Parameters.Add(min);
                cmd.Parameters.Add(max);

                var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                var rowCount = 0;
                var fieldArray = _output.Entity.Fields.ToArray();
                while (reader.Read()) {
                    rowCount++;
                    _output.Increment();
                    yield return _rowCreator.Create(reader, fieldArray);
                }
                _output.Info("{0} from {1}", rowCount, _output.Connection.Name);
            }
        }
    }
}