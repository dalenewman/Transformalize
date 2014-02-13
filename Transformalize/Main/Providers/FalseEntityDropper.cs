namespace Transformalize.Main.Providers
{
    public class FalseEntityDropper : IEntityDropper
    {
        public IEntityExists EntityExists { get; set; }
        public void Drop(AbstractConnection connection, Entity entity)
        {
            //never dropping anything
        }
    }
}