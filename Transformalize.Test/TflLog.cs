using Transformalize.Main;

namespace Transformalize.Test {
    public class TflLog : TflNode {
        public TflLog() {
            Key("name");
            Property("provider", Common.DefaultValue);
            Property("layout", Common.DefaultValue);
            Property("level", "Informational");
            Property("connection", Common.DefaultValue);
            Property("from", Common.DefaultValue);
            Property("to", Common.DefaultValue);
            Property("subject", Common.DefaultValue);
            Property("file", Common.DefaultValue);
            Property("folder", Common.DefaultValue);
            Property("async", false);
        }
    }
}