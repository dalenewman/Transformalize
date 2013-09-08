namespace Transformalize.Providers
{
    public class Compatibility
    {
        public bool CanInsertMultipleRows { get; set; }
        public bool SupportsTop { get; set; }
        public bool SupportsLimit { get; set; }
        public bool SupportsNoLock { get; set; }
    }
}