namespace Transformalize.Main.Providers.Internal
{
    public class InternalConnectionChecker : IConnectionChecker
    {
        public bool Check(AbstractConnection connection)
        {
            return connection.InputOperation != null;
        }
    }
}