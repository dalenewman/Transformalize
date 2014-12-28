using Transformalize.Main;

namespace Transformalize.Test {
    public class TflConnection : TflNode {
        public TflConnection() {
            Key("name");

            Property("batch-size", 500);
            Property("connection-string", string.Empty);
            Property("content-type", string.Empty);
            Property("data", Common.DefaultValue);
            Property("database", string.Empty);
            Property("date-format", "MM/dd/yyyy h:mm:ss tt");
            Property("delimiter", ",");
            Property("direct", false);
            Property("enabled", true);
            Property("enable-ssl", false);
            Property("encoding", "utf-8");
            Property("end", 0);
            Property("error-mode", string.Empty);
            Property("file", string.Empty);
            Property("folder", string.Empty);
            Property("footer", string.Empty);
            Property("header", Common.DefaultValue);
            Property("password", string.Empty);
            Property("path", string.Empty);
            Property("port", 0);
            Property("provider", "SqlServer");
            Property("search-option", "TopDirectoryOnly");
            Property("search-pattern", "*.*");
            Property("server", "localhost");
            Property("start", 1);
            Property("url", string.Empty);
            Property("user", string.Empty);
            Property("version", Common.DefaultValue);
            Property("web-method", "GET");
        }
    }
}