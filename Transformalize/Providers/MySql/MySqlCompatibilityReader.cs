namespace Transformalize.Providers.MySql
{
    public class MySqlCompatibilityReader : ICompatibilityReader
    {
        public Compatibility Read(IConnection connection)
        {
            return new Compatibility {CanInsertMultipleRows = true};
        }
    }
}