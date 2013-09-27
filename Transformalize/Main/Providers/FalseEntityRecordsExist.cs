namespace Transformalize.Main.Providers
{
    public class FalseEntityRecordsExist : IEntityRecordsExist {
        public bool RecordsExist(AbstractConnection connection, string schema, string name)
        {
            return false;
        }
    }
}