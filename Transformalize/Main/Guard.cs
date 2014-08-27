namespace Transformalize.Main {
    public class Guard {
        public static void Against(bool assertion, string message, params object[] args) {
            if (assertion == false)
                return;
            throw new TransformalizeException(message, args);
        }
    }
}
