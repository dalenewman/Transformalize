using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityCreate : AbstractOperation {
        private readonly Entity _entity;
        private readonly IEntityExists _entityExists;
        private readonly FieldSqlWriter _writer;

        public EntityCreate(Entity entity, Process process, IEntityExists entityExists = null) {
            _entity = entity;
            _writer = entity.IsMaster() ?
                new FieldSqlWriter(entity.All, process.Results, process.RelatedKeys.ToDictionary(k => k.Alias, v => v)) :
                new FieldSqlWriter(entity.All);
            _entityExists = entityExists ?? new SqlServerEntityExists();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            CreateEntity();
            return rows;
        }

        private void CreateEntity() {
            if (_entityExists.OutputExists(_entity)) return;

            var primaryKey = _writer.FieldType(FieldType.MasterKey, FieldType.PrimaryKey).Alias().Asc().Values();
            var defs = _writer.Reload().ExpandXml().AddSurrogateKey().AddBatchId().Output().Alias().DataType().AppendIf(" NOT NULL", FieldType.MasterKey, FieldType.PrimaryKey).Values();
            var sql = SqlTemplates.CreateTable(_entity.OutputName(), defs, primaryKey, ignoreDups: true);

            Debug(sql);

            using (var cn = new SqlConnection(_entity.OutputConnection.ConnectionString)) {
                cn.Open();
                var cmd = new SqlCommand(sql, cn);
                cmd.ExecuteNonQuery();
                Info("{0} | Created Output {1}.{2}", _entity.ProcessName, _entity.Schema, _entity.OutputName());
            }
        }
    }
}