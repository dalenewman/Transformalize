namespace Transformalize.Main.Providers {
    public class FalseConnectionChecker : IConnectionChecker {
        public bool Check(AbstractConnection connection) {
            return true;
        }
    }
}