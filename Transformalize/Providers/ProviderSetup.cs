namespace Transformalize.Providers
{
    public class ProviderSetup
    {
        public string ProviderType { get; set; }
        public string L { get; set; }
        public string R { get; set; }
        public string Enclose(string name)
        {
            return L + name + R;
        }
    }
}