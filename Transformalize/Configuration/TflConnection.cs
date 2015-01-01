using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflConnection : CfgNode {

        public TflConnection() {
            Property(n: "name", v: string.Empty, r: true, u: true);
            Property(n: "batch-size", v: 500);
            Property(n: "connection-string", v: string.Empty);
            Property(n: "content-type", v: string.Empty);
            Property(n: "data", v: Common.DefaultValue);
            Property(n: "database", v: string.Empty);
            Property(n: "date-format", v: "MM/dd/yyyy h:mm:ss tt");
            Property(n: "delimiter", v: ',');
            Property(n: "direct", v: false);
            Property(n: "enabled", v: true);
            Property(n: "enable-ssl", v: false);
            Property(n: "encoding", v: "utf-8");
            Property(n: "end", v: 0);
            Property(n: "error-mode", v: string.Empty);
            Property(n: "file", v: string.Empty);
            Property(n: "folder", v: string.Empty);
            Property(n: "footer", v: string.Empty);
            Property(n: "header", v: Common.DefaultValue);
            Property(n: "password", v: string.Empty);
            Property(n: "path", v: string.Empty);
            Property(n: "port", v: 0);
            Property(n: "provider", v: "SqlServer");
            Property(n: "search-option", v: "TopDirectoryOnly");
            Property(n: "search-pattern", v: "*.*");
            Property(n: "server", v: "localhost");
            Property(n: "start", v: 1);
            Property(n: "url", v: string.Empty);
            Property(n: "user", v: string.Empty);
            Property(n: "version", v: Common.DefaultValue);
            Property(n: "web-method", v: "GET");
        }

        public string Name { get; set; }
        public int BatchSize { get; set; }
        public string ConnectionString { get; set; }
        public string ContentType { get; set; }
        public string Data { get; set; }
        public string DataBase { get; set; }
        public string DateFormat { get; set; }
        public char Delimiter { get; set; }
        public bool Direct { get; set; }
        public bool Enabled { get; set; }
        public bool EnableSsl { get; set; }
        public string Encoding { get; set; }
        public int End { get; set; }
        public string ErrorMode { get; set; }
        public string File { get; set; }
        public string Folder { get; set; }
        public string Footer { get; set; }
        public string Header { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public int Port { get; set; }
        public string Provider { get; set; }
        public string SearchOption { get; set; }
        public string SearchPattern { get; set; }
        public string Server { get; set; }
        public int Start { get; set; }
        public string Url { get; set; }
        public string User { get; set; }
        public string Version { get; set; }
        public string WebMethod { get; set; }

    }
}