namespace Transformalize.Providers.MySql
{
    public class MySqlCompatibilityReader : ICompatibilityReader
    {
        public bool CanInsertMultipleValues { get; private set; }

        public MySqlCompatibilityReader()
        {
            CanInsertMultipleValues = true;
        }
    }
}