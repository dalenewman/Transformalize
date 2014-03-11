namespace Transformalize.Main.Providers
{
    public class FalseEntityCreator : IEntityCreator {
        public IEntityExists EntityExists { get; set; }
        public void Create(AbstractConnection connection, Process process, Entity entity) {
            //never create anything
        }
    }
}