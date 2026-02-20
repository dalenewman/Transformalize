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
using Transformalize.Contracts;
using Transformalize.Providers.Ado;

namespace Transformalize.Providers.PostgreSql {

   /// <summary>
   /// Can't use ADO version of this query because PostgreSql does the UPDATE FROM stuff a little differently
   /// You UPDATE theTarget t FROM AnotherTable a WHERE t.Key = a.Key, you do not specify the target table twice like in T-SQL 
   /// </summary>
   public class PostgreSqlUpdateMasterKeysQueryWriter : IWriteMasterUpdateQuery {
      private readonly IContext _c;
      private readonly IConnectionFactory _cf;
      public PostgreSqlUpdateMasterKeysQueryWriter(IContext context, IConnectionFactory factory) {
         _c = context;
         _cf = factory;
      }

      public string Write(EntityStatus status) {
         /* if update occurs, outside of first run, you must update the master's 
            batch id to the entity's batch id (which will be higher).
            Updating this clues us in that we have to update process-level calculated columns
            on the master record.  It also keeps the master's batch id incrementing to indicate 
            updates for subsequent processes using this process' output as an input with TflBatchId 
            as a version field.
         */

         var masterEntity = _c.Process.Entities.First(e => e.IsMaster);
         var masterTable = _cf.Enclose(masterEntity.OutputTableName(_c.Process.Name));
         var masterAlias = masterEntity.GetExcelName();

         var entityAlias = _c.Entity.GetExcelName();
         var builder = new StringBuilder();
         var setPrefix = string.Empty;

         var sets = _c.Entity.Fields.Any(f => f.KeyType.HasFlag(KeyType.Foreign)) ?
             string.Join(",", _c.Entity.Fields
                 .Where(f => f.KeyType.HasFlag(KeyType.Foreign))
                 .Select(f => $"{setPrefix}{_cf.Enclose(f.FieldName())} = {entityAlias}.{_cf.Enclose(f.FieldName())}"))
             :
             string.Empty;

         builder.AppendLine();
         builder.AppendLine($"UPDATE {masterTable} {masterAlias}");
         builder.AppendLine($"SET {sets}{(sets == string.Empty ? string.Empty : ", ")}{_cf.Enclose(masterEntity.TflBatchId().FieldName())} = @TflBatchId");

         var relationships = _c.Entity.RelationshipToMaster.Reverse().ToArray();
         var tables = string.Join(", ", relationships.Select(r => _cf.Enclose(r.Summary.RightEntity.OutputTableName(_c.Process.Name)) + " " + r.Summary.RightEntity.GetExcelName()));

         builder.AppendLine($"FROM {tables}");

         for (var r = 0; r < relationships.Length; r++) {

            var relationship = relationships[r];
            var rightEntityAlias = relationship.Summary.RightEntity.GetExcelName();

            builder.Append(r == 0 ? "WHERE (" : "AND (");

            var leftEntityAlias = relationship.Summary.LeftEntity.GetExcelName();
            for (var i = 0; i < relationship.Summary.LeftFields.Count; i++) {
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

         builder.AppendLine($"AND ({entityAlias}.{_cf.Enclose(_c.Entity.TflBatchId().FieldName())} = @TflBatchId OR {masterAlias}.{_cf.Enclose(masterEntity.TflBatchId().FieldName())} >= @MasterTflBatchId)");

         var sql = builder.ToString();
         _c.Debug(() => sql);
         return sql;

      }
   }
}
