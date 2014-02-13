namespace Transformalize.Main.Providers.SqlCe4 {
    public class SqlCe4EntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            return new SqlCe4TableExists(connection).Exists(entity.Schema, entity.OutputName());
        }
    }
}