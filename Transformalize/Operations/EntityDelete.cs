using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class EntityDelete : SqlBatchOperation
    {
        private readonly Entity _entity;

        public EntityDelete(Process process, Entity entity) : base(process.OutputConnection)
        {
            _entity = entity;
        }

        protected override void PrepareCommand(Row row, SqlCommand command)
        {
            command.CommandText = string.Format("DELETE FROM [{0}] WHERE TflKey = @TflKey;", _entity.OutputName());
            AddParameter(command, "@TflKey", row["TflKey"]);
        }
    }
}