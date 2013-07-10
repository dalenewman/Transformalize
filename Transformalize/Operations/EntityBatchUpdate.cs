using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityBatchUpdate : SqlBatchOperation {
        private readonly Entity _entity;
        
        public EntityBatchUpdate(Entity entity)
            : base(entity.OutputConnection.ConnectionString) {
            _entity = entity;
            BatchSize = 50;
            UseTransaction = false;
        }

        protected override void PrepareCommand(Row row, SqlCommand command)
        {
            var writer = new FieldSqlWriter(_entity.Fields).ExpandXml().Output();
            var sets = writer.Alias().SetParam().Write(",\r\n    ", false);
            command.CommandText = string.Format("UPDATE [{0}].[{1}]\r\nSET {2},\r\n    TflBatchId = @TflBatchId\r\nWHERE TflKey = @TflKey;", _entity.Schema, _entity.OutputName(), sets);
            foreach (var r in writer.Context()) {
                AddParameter(command, r.Key, row[r.Key]);
            }
            AddParameter(command, "TflKey", row["TflKey"]);
            AddParameter(command, "TflBatchId", _entity.TflBatchId);

            Debug(command.CommandText);
        }
    }
}