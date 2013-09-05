namespace Transformalize.Providers.MySql
{
    public class MySqlCompatibilityReader : ICompatibilityReader
    {
        public bool InsertMultipleValues { get; private set; }

        public MySqlCompatibilityReader()
        {
            InsertMultipleValues = true;
        }
    }
}