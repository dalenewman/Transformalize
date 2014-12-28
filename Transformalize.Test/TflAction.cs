namespace Transformalize.Test {
    public class TflAction : TflNode {
        public TflAction() {
            Property("action", string.Empty, true);
            Property("after", true);
            Property("arguments", string.Empty);
            Property("bcc", string.Empty);
            Property("before", false);
            Property("body", string.Empty);
            Property("cc", string.Empty);
            Property("command", string.Empty);
            Property("conditional", false);
            Property("connection", string.Empty);
            Property("file", string.Empty);
            Property("from", string.Empty);
            Property("html", true);
            Property("method", "get");
            Property("mode", "*");
            Property("new-value", string.Empty);
            Property("old-value", string.Empty);
            Property("subject", string.Empty);
            Property("time-out", 0);
            Property("to", string.Empty);
            Property("url", string.Empty);

            Class<TflNameReference>("modes");
        }
    }
}