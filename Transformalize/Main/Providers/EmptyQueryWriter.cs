namespace Transformalize.Main.Providers
{
    public class EmptyQueryWriter : IEntityQueryWriter {
        public string Write(Entity entity) {
            return string.Empty;
        }
    }
}