namespace Transformalize.Main.Providers.SqlCe4
{
    public class SqlCe4EntityRecordsExist : IEntityRecordsExist {
        private readonly IEntityExists _entityExists;

        public SqlCe4EntityRecordsExist() {
            _entityExists = new SqlCe4EntityExists();
        }

        public bool RecordsExist(AbstractConnection connection, Entity entity) {

            if (_entityExists.Exists(connection, entity)) {

                using (var cn = connection.GetConnection()) {
                    cn.Open();
                    var sql = string.Format(@"SELECT [{0}] FROM [{1}];", entity.PrimaryKey.First().Key, entity.OutputName());
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = sql;
                    using (var reader = cmd.ExecuteReader()) {
                        return reader.Read();
                    }
                }
            }
            return false;
        }
    }
}