using System;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflConnection : CfgNode {
        private readonly char[] _slash = { '/' };
        private string _provider;

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
        [Cfg(value = "MM/dd/yyyy h:mm:ss tt")]
        public string DateFormat { get; set; }

        [Cfg(value = ",")]
        public string Delimiter { get; set; }

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
        [Cfg(value = "SaveAndContinue", domain = "ThrowException,SaveAndContinue,IgnoreAndContinue", ignoreCase = true)]
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

        [Cfg(value = "sqlserver", domain = "sqlserver,mysql,postgresql,sqlce,analysisservices,file,folder,internal,console,log,mail,html,elasticsearch,solr,lucene,web", ignoreCase = true)]
        public string Provider {
            get { return _provider; }
            set { _provider = value == null ? string.Empty : value.ToLower(); }
        }

        [Cfg(value = "TopDirectoryOnly", domain = "AllDirectories,TopDirectoryOnly", ignoreCase = true)]
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
        [Cfg(value = true)]
        public bool Check { get; set; }

        protected override void Validate() {
            const StringComparison ic = StringComparison.OrdinalIgnoreCase;
            if (Provider.Equals("File", ic) && string.IsNullOrEmpty(File)) {
                AddProblem("The file provider requires a file.");
            } else if (Provider.Equals("Folder", ic) && string.IsNullOrEmpty(Folder)) {
                AddProblem("The folder provider requires a folder.");
            }

            if (Delimiter.Length > 1) {
                AddProblem(string.Format("Invalid delimiter defined for connection '{0}'.  The delimiter '{1}' is too long.  It can only be zero or one character.", Name, Delimiter));
            }
        }

        public string NormalizeUrl(int defaultPort) {
            var builder = new UriBuilder(Server);
            if (Port > 0) {
                builder.Port = Port;
            }
            if (builder.Port == 0) {
                builder.Port = defaultPort;
            }
            if (!Path.Equals(string.Empty) && Path != builder.Path) {
                builder.Path = builder.Path.TrimEnd(_slash) + "/" + Path.TrimStart(_slash);
            } else if (!Folder.Equals(string.Empty) && Folder != builder.Path) {
                builder.Path = builder.Path.TrimEnd(_slash) + "/" + Folder.TrimStart(_slash);
            }
            return builder.ToString();
        }
    }
}