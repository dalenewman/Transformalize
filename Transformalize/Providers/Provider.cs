namespace Transformalize.Providers
{
    public abstract class AbstractProvider
    {
        public ProviderSupports Supports { get; set; }
        public ProviderType Type { get; set; }
        public string L { get; set; }
        public string R { get; set; }

        public string Enclose(string name)
        {
            return L + name + R;
        }
    }
}