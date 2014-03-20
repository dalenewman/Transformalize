namespace Transformalize.Main.Providers
{
    public class EmptyQueryWriter : IEntityQueryWriter {
        public string Write(Entity entity, AbstractConnection connection) {
            return string.Empty;
        }
    }
}