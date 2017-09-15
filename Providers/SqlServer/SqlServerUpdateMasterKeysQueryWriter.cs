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
using System.Linq;
using System.Text;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.SqlServer {
    public class SqlServerUpdateMasterKeysQueryWriter : IWriteMasterUpdateQuery {
        private readonly PipelineContext _c;
        private readonly IConnectionFactory _cf;

        public SqlServerUpdateMasterKeysQueryWriter(
            PipelineContext context,
            IConnectionFactory factory
        ) {
            _c = context;
            _cf = factory;
        }

        public string Write(EntityStatus status) {
            /* if update occurs, outside of first run, you must update the master's 
               batch id to the entity's batch id (which will be higher).
               Updating this indicates a change and we have to update process-level calculated columns
               on the master record.
               
               It also keeps the master's batch id incrementing to indicate 
               updates for subsequent processes using this process' output as an input with TflBatchId 
               as a version field.
            */

            var masterEntity = _c.Process.Entities.First(e => e.IsMaster);
            var masterTable = _cf.Enclose(masterEntity.OutputTableName(_c.Process.Name));
            var masterAlias = masterEntity.GetExcelName();

            var entityAlias = _c.Entity.GetExcelName();
            var builder = new StringBuilder();
            builder.AppendLine($"UPDATE {masterAlias}");
            builder.Append("SET ");

            var setPrefix = masterAlias + ".";

            foreach (var field in _c.Entity.Fields.Where(f => f.KeyType.HasFlag(KeyType.Foreign))) {
                builder.AppendLine($"{setPrefix}{_cf.Enclose(field.FieldName())} = {entityAlias}.{_cf.Enclose(field.FieldName())},");
            }

            builder.AppendLine($"{setPrefix}{_cf.Enclose(masterEntity.TflBatchId().FieldName())} = @TflBatchId");

            builder.AppendFormat(" FROM {0} {1}", masterTable, masterAlias);

            var relationships = _c.Entity.RelationshipToMaster.Reverse().ToArray();

            for (var r = 0; r < relationships.Length; r++) {
                var relationship = relationships[r];
                var right = _cf.Enclose(relationship.Summary.RightEntity.OutputTableName(_c.Process.Name));
                var rightEntityAlias = relationship.Summary.RightEntity.GetExcelName();

                builder.AppendFormat(" INNER JOIN {0} {1} ON ( ", right, rightEntityAlias);

                var leftEntityAlias = relationship.Summary.LeftEntity.GetExcelName();
                for (var i = 0; i < relationship.Summary.LeftFields.Count(); i++) {
                    var leftAlias = relationship.Summary.LeftFields[i].FieldName();
                    var rightAlias = relationship.Summary.RightFields[i].FieldName();
                    var conjunction = i > 0 ? " AND " : string.Empty;
                    builder.AppendFormat(
                        "{0}{1}.{2} = {3}.{4}",
                        conjunction,
                        leftEntityAlias,
                        _cf.Enclose(leftAlias),
                        rightEntityAlias,
                        _cf.Enclose(rightAlias)
                    );
                }
                builder.AppendLine(")");
            }

            builder.Append("WHERE ");

            if (status.Modified) {
                builder.AppendLine($"{entityAlias}.{_cf.Enclose(_c.Entity.TflBatchId().FieldName())} = @TflBatchId ");
                if (status.MasterUpserted && status.HasForeignKeys) {
                    builder.AppendLine($"OR {masterAlias}.{_cf.Enclose(masterEntity.TflBatchId().FieldName())} >= @MasterTflBatchId");
                }
            } else if (status.MasterUpserted && status.HasForeignKeys) {
                builder.AppendLine($"{masterAlias}.{_cf.Enclose(masterEntity.TflBatchId().FieldName())} >= @MasterTflBatchId");
            }

            var sql = builder.ToString();
            _c.Debug(() => sql);
            return sql;
        }

    }
}

