namespace Transformalize.Providers
{
    public interface IProviderSupportsModifier
    {
        void Modify(AbstractConnection connection, ProviderSupports supports);
    }
}