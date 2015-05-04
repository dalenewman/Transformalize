namespace Transformalize.Main.Providers {
    public class NullConnectionChecker : IConnectionChecker {
        public bool Check(AbstractConnection connection) {
            return true;
        }
    }
}