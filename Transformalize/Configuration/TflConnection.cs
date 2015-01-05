using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflConnection : CfgNode {

        public TflConnection() {
            Property(name: "name", value: string.Empty, required: true, unique: true);
            Property(name: "batch-size", value: 500);
            Property(name: "connection-string", value: string.Empty);
            Property(name: "content-type", value: string.Empty);
            Property(name: "data", value: Common.DefaultValue);
            Property(name: "database", value: string.Empty);
            Property(name: "date-format", value: "MM/dd/yyyy h:mm:ss tt");
            Property(name: "delimiter", value: ',');
            Property(name: "direct", value: false);
            Property(name: "enabled", value: true);
            Property(name: "enable-ssl", value: false);
            Property(name: "encoding", value: "utf-8");
            Property(name: "end", value: 0);
            Property(name: "error-mode", value: string.Empty);
            Property(name: "file", value: string.Empty);
            Property(name: "folder", value: string.Empty);
            Property(name: "footer", value: string.Empty);
            Property(name: "header", value: Common.DefaultValue);
            Property(name: "password", value: string.Empty);
            Property(name: "path", value: string.Empty);
            Property(name: "port", value: 0);
            Property(name: "provider", value: "SqlServer");
            Property(name: "search-option", value: "TopDirectoryOnly");
            Property(name: "search-pattern", value: "*.*");
            Property(name: "server", value: "localhost");
            Property(name: "start", value: 1);
            Property(name: "url", value: string.Empty);
            Property(name: "user", value: string.Empty);
            Property(name: "version", value: Common.DefaultValue);
            Property(name: "web-method", value: "GET");
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