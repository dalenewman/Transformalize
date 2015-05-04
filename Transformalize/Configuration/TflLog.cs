using Transformalize.Extensions;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Configuration {
    public class TflLog : CfgNode {
        private string _level;

        [Cfg(value = false)]
        public bool Async { get; set; }

        [Cfg(value = Common.DefaultValue, toLower = true)]
        public string Connection { get; set; }

        [Cfg(value = Common.DefaultValue)]
        public string File { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Folder { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string From { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Layout { get; set; }

        [Cfg(value = "Informational", domain = "Informational,Error,Verbose,Warning", ignoreCase = true)]
        public string Level {
            get { return _level; }
            set {
                if (value == null)
                    return;
                var level = value.ToLower().Left(4);
                switch (level) {
                    case "verb":
                        _level = "Verbose";
                        break;
                    case "debu":
                        goto case "verb";
                    case "info":
                        _level = "Informational";
                        break;
                    case "warn":
                        _level = "Warning";
                        break;
                    case "erro":
                        _level = "Error";
                        break;
                }
            }
        }

        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }

        [Cfg(value = "file", domain = "file,console,mail", ignoreCase = true)]
        public string Provider { get; set; }

        [Cfg(value = Common.DefaultValue)]
        public string Subject { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string To { get; set; }
        [Cfg(value = (long)10000)]
        public long Rows { get; set; }

        public AbstractConnection ConnectionInstance { get; set; }
    }
}