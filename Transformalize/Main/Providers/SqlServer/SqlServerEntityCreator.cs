using Transformalize.Libs.Dapper;

namespace Transformalize.Main.Providers.SqlServer {

    public class SqlServerEntityCreator : DatabaseEntityCreator {

        public override void Create(AbstractConnection connection, Process process, Entity entity) {

            if (EntityExists != null && EntityExists.Exists(connection, entity)) {
                Log.Warn("Trying to create entity that already exists! {0}", entity.Name);
                return;
            }

            var writer = entity.IsMaster() ?
                new FieldSqlWriter(entity.Fields, process.CalculatedFields, entity.CalculatedFields, GetRelationshipFields(process.Relationships, entity)) :
                new FieldSqlWriter(entity.Fields, entity.CalculatedFields);

            var primaryKey = writer.FieldType(entity.IsMaster() ? FieldType.MasterKey : FieldType.PrimaryKey).Alias(connection.L, connection.R).Asc().Values();
            var defs = writer.Reload().AddSurrogateKey().AddBatchId().Output().Alias(connection.L, connection.R).DataType(new SqlServerDataTypeService()).AppendIf(" NOT NULL", entity.IsMaster() ? FieldType.MasterKey : FieldType.PrimaryKey).Values();

            var createSql = connection.TableQueryWriter.CreateTable(entity.OutputName(), defs);
            Log.Debug(createSql);

            var indexSql = connection.TableQueryWriter.AddUniqueClusteredIndex(entity.OutputName());
            Log.Debug(indexSql);

            var keySql = connection.TableQueryWriter.AddPrimaryKey(entity.OutputName(), primaryKey);
            Log.Debug(keySql);

            using (var cn = connection.GetConnection()) {
                cn.Open();
                cn.Execute(createSql);
                cn.Execute(indexSql);
                cn.Execute(keySql);
                Log.Info("Initialized {0} in {1} on {2}.", entity.OutputName(), connection.Database, connection.Server);
            }
        }
    }
}