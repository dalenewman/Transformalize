using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflAction : CfgNode {

        [Cfg(required = true)]
        public string Action { get; set; }
        [Cfg(value = true)]
        public bool After { get; set; }
        [Cfg(value = "")]
        public string Arguments { get; set; }
        [Cfg(value = "")]
        public string Bcc { get; set; }
        [Cfg(value = false)]
        public bool Before { get; set; }
        [Cfg(value = "")]
        public string Body { get; set; }
        [Cfg(value = "")]
        public string Cc { get; set; }
        [Cfg(value = "")]
        public string Command { get; set; }
        [Cfg(value = "")]
        public string Connection { get; set; }
        [Cfg(value = "")]
        public string File { get; set; }
        [Cfg(value = "")]
        public string From { get; set; }
        [Cfg(value = true)]
        public bool Html { get; set; }
        [Cfg(value = "get")]
        public string Method { get; set; }
        [Cfg(value = "*")]
        public string Mode { get; set; }
        [Cfg(value = "")]
        public string NewValue { get; set; }
        [Cfg(value = "")]
        public string OldValue { get; set; }
        [Cfg(value = "")]
        public string Subject { get; set; }
        [Cfg(value = 0)]
        public int TimeOut { get; set; }
        [Cfg(value = "")]
        public string To { get; set; }
        [Cfg(value = "")]
        public string Url { get; set; }

        public List<TflNameReference> Modes { get; set; }

        public string[] GetModes() {
            return this.Count("modes") > 0 ? Modes.Select(m => m.Name).ToArray() : new[] { Mode };
        }
    }
}