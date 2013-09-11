namespace Transformalize.Providers
{
    public class DefaultProviderSupportsModifier : IProviderSupportsModifier
    {
        public void Modify(AbstractConnection connection, ProviderSupports supports)
        {
            //do not modify supports
        }
    }
}