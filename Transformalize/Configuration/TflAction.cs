using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflAction : CfgNode {

        public TflAction() {

            Property(name: "action", value: string.Empty, required: true);
            Property(name: "after", value: true);
            Property(name: "arguments", value: string.Empty);
            Property(name: "bcc", value: string.Empty);
            Property(name: "before", value: false);
            Property(name: "body", value: string.Empty);
            Property(name: "cc", value: string.Empty);
            Property(name: "command", value: string.Empty);
            Property(name: "connection", value: string.Empty);
            Property(name: "file", value: string.Empty);
            Property(name: "from", value: string.Empty);
            Property(name: "html", value: true);
            Property(name: "method", value: "get");
            Property(name: "mode", value: "*");
            Property(name: "new-value", value: string.Empty);
            Property(name: "old-value", value: string.Empty);
            Property(name: "subject", value: string.Empty);
            Property(name: "time-out", value: 0);
            Property(name: "to", value: string.Empty);
            Property(name: "url", value: string.Empty);

            Collection<TflNameReference>("modes");
        }

        public string Action { get; set; }
        public bool After { get; set; }
        public string Arguments { get; set; }
        public string Bcc { get; set; }
        public bool Before { get; set; }
        public string Body { get; set; }
        public string Cc { get; set; }
        public string Command { get; set; }
        public string Connection { get; set; }
        public string File { get; set; }
        public string From { get; set; }
        public bool Html { get; set; }
        public string Method { get; set; }
        public string Mode { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string Subject { get; set; }
        public int TimeOut { get; set; }
        public string To { get; set; }
        public string Url { get; set; }

        public List<TflNameReference> Modes { get; set; }
    }
}