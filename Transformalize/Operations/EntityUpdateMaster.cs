using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityUpdateMaster : AbstractOperation {
        private readonly Process _process;
        private readonly Entity _entity;

        public EntityUpdateMaster(Process process, Entity entity) {
            _process = process;
            _entity = entity;
            UseTransaction = false;
        }


        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            if (!_entity.IsMaster() && _entity.HasForeignKeys()) {
                using (var cn = new SqlConnection(_process.MasterEntity.OutputConnection.ConnectionString)) {
                    cn.Open();
                    var sql = PrepareSql();
                    var cmd = new SqlCommand(sql, cn);
                    var records = cmd.ExecuteNonQuery();
                    Info("{0} | Updated {1} Master Records.", _process.Name, records);
                }
            }

            return rows;
        }

        private string PrepareSql() {
            var builder = new StringBuilder();
            var master = _process.Entities.Where(e => e.Value.IsMaster())
                                 .Select(e => string.Format("[{0}].[{1}]", e.Value.Schema, e.Value.OutputName()))
                                 .First();
            var source = string.Format("[{0}].[{1}]", _entity.Schema, _entity.OutputName());
            var sets = new FieldSqlWriter(_entity.Fields).FieldType(FieldType.ForeignKey).Alias().Set(master, source);

            builder.AppendFormat("UPDATE {0}\r\n", master);
            builder.AppendFormat("SET {0}\r\n", sets);
            builder.AppendFormat("FROM [{0}].[{1}]\r\n", _entity.Schema, _entity.OutputName());

            foreach (var relationship in _entity.RelationshipToMaster) {
                var left = string.Format("[{0}].[{1}]", relationship.LeftEntity.Schema, relationship.LeftEntity.OutputName());
                var right = string.Format("[{0}].[{1}]", relationship.RightEntity.Schema, relationship.RightEntity.OutputName());
                var join = string.Join(" AND ", relationship.Join.Select(j => string.Format("{0}.[{1}] = {2}.[{3}]", left, j.LeftField.Alias, right, j.RightField.Alias)));
                builder.AppendFormat("INNER JOIN {0} ON ({1})\r\n", left, join);
            }

            return builder.ToString();
        }
    }
}