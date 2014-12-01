namespace Transformalize.Main.Providers.SqlCe {

    public class SqlCeEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            return new SqlCeTableExists(connection).OutputExists(entity.OutputName());
        }
    }
}