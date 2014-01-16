namespace Transformalize.Main.Providers
{
    public class FalseEntityDropper : IEntityDropper
    {
        public IEntityExists EntityExists { get; set; }
        public void Drop(AbstractConnection connection, string schema, string name)
        {
            //never dropping anything
        }
    }
}