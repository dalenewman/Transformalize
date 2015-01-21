using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflConnection : CfgNode {
        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(value = 500)]
        public int BatchSize { get; set; }
        [Cfg(value = "")]
        public string ConnectionString { get; set; }
        [Cfg(value = "")]
        public string ContentType { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Data { get; set; }
        [Cfg(value = "")]
        public string Database { get; set; }
        [Cfg(value = "MM/dd/yyyy h=mm=ss tt")]
        public string DateFormat { get; set; }
        [Cfg(value = ',')]
        public char Delimiter { get; set; }
        [Cfg(value = false)]
        public bool Direct { get; set; }
        [Cfg(value = true)]
        public bool Enabled { get; set; }
        [Cfg(value = false)]
        public bool EnableSsl { get; set; }
        [Cfg(value = "utf-8")]
        public string Encoding { get; set; }
        [Cfg(value = 0)]
        public int End { get; set; }
        [Cfg(value = "")]
        public string ErrorMode { get; set; }
        [Cfg(value = "")]
        public string File { get; set; }
        [Cfg(value = "")]
        public string Folder { get; set; }
        [Cfg(value = "")]
        public string Footer { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Header { get; set; }
        [Cfg(value = "")]
        public string Password { get; set; }
        [Cfg(value = "")]
        public string Path { get; set; }
        [Cfg(value = 0)]
        public int Port { get; set; }
        [Cfg(value = "SqlServer")]
        public string Provider { get; set; }
        [Cfg(value = "TopDirectoryOnly")]
        public string SearchOption { get; set; }
        [Cfg(value = "*.*")]
        public string SearchPattern { get; set; }
        [Cfg(value = "localhost")]
        public string Server { get; set; }
        [Cfg(value = 1)]
        public int Start { get; set; }
        [Cfg(value = "")]
        public string Url { get; set; }
        [Cfg(value = "")]
        public string User { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Version { get; set; }
        [Cfg(value = "GET")]
        public string WebMethod { get; set; }
    }
}