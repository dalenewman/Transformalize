using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflLog : CfgNode {

        [Cfg(value = false)]
        public bool Async { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Connection { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string File { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Folder { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string From { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Layout { get; set; }
        [Cfg(value = "Informational")]
        public string Level { get; set; }
        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Provider { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Subject { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string To { get; set; }

    }
}