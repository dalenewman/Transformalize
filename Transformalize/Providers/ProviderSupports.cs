namespace Transformalize.Providers
{
    public class ProviderSupports
    {
        public bool InsertMultipleRows { get; set; }
        public bool Top { get; set; }
        public bool NoLock { get; set; }
        public bool TableVariable { get; set; }
        public bool NoCount { get; set; }
        public bool MaxDop { get; set; }
    }
}