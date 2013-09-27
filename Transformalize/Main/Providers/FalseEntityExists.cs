namespace Transformalize.Main.Providers
{
    public class FalseEntityExists : IEntityExists
    {
        public bool Exists(AbstractConnection connection, string schema, string name)
        {
            return false;
        }
    }
}