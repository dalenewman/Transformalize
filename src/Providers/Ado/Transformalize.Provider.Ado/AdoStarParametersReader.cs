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

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
    public class AdoStarParametersReader : IRead {

        private readonly IConnectionContext _output;
        private readonly Process _parent;
        private readonly IConnectionFactory _cf;
        private readonly AdoRowCreator _rowCreator;

        public AdoStarParametersReader(IConnectionContext output, Process parent, IConnectionFactory cf, IRowFactory rowFactory) {
            _output = output;
            _parent = parent;
            _cf = cf;
            _rowCreator = new AdoRowCreator(output, rowFactory);
        }

        public IEnumerable<IRow> Read() {

            if (_parent.Entities.Sum(e => e.Inserts + e.Updates + e.Deletes) == 0) {
                yield break;
            };

            var batches = _parent.Entities.Select(e => e.BatchId).ToArray();
            var minBatchId = batches.Min();
            var maxBatchId = batches.Max();
            _output.Info("Batch Range: {0} to {1}.", minBatchId, maxBatchId);

            var threshold = minBatchId - 1;

            var sql = string.Empty;

            if (_cf.AdoProvider == AdoProvider.SqlCe) {

                // because SqlCe doesn't support views, re-construct the parent view's definition

                var ctx = new PipelineContext(_output.Logger, _parent);
                var master = _parent.Entities.First(e => e.IsMaster);
                var builder = new StringBuilder();

                builder.AppendLine($"SELECT {string.Join(",", _output.Entity.Fields.Where(f => f.Output).Select(f => _cf.Enclose(f.Source.Split('.')[0]) + "." + _cf.Enclose(f.Source.Split('.')[1])))}");
                foreach (var from in ctx.SqlStarFroms(_cf)) {
                    builder.AppendLine(@from);
                }
                builder.AppendLine($"WHERE {_cf.Enclose(Utility.GetExcelName(master.Index))}.{_cf.Enclose(master.TflBatchId().FieldName())} > @Threshold;");

                sql = builder.ToString();

            } else {
                sql = $@"
                SELECT {string.Join(",", _output.Entity.Fields.Where(f => f.Output).Select(f => _cf.Enclose(f.Alias)))} 
                FROM {_cf.Enclose(_output.Process.Name + _output.Process.StarSuffix)} {(_cf.AdoProvider == AdoProvider.SqlServer ? "WITH (NOLOCK)" : string.Empty)} 
                WHERE {_cf.Enclose(Constants.TflBatchId)} > @Threshold;";
            }

            _output.Debug(() => sql);

            using (var cn = _cf.GetConnection()) {
                cn.Open();

                var cmd = cn.CreateCommand();

                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                var min = cmd.CreateParameter();
                min.ParameterName = "@Threshold";
                min.Value = threshold;
                min.Direction = ParameterDirection.Input;
                min.DbType = DbType.Int32;

                cmd.Parameters.Add(min);

                var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                var rowCount = 0;
                var fieldArray = _output.Entity.Fields.ToArray();
                while (reader.Read()) {
                    rowCount++;
                    yield return _rowCreator.Create(reader, fieldArray);
                }
                _output.Info("{0} from {1}", rowCount, _output.Connection.Name);
            }
        }
    }
}