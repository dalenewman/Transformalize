using System.Collections.Generic;

namespace Transformalize.Configuration {
    public class TflAction : CfgNode {

        public TflAction() {

            Property(n: "action", v: string.Empty, r: true);
            Property(n: "after", v: true);
            Property(n: "arguments", v: string.Empty);
            Property(n: "bcc", v: string.Empty);
            Property(n: "before", v: false);
            Property(n: "body", v: string.Empty);
            Property(n: "cc", v: string.Empty);
            Property(n: "command", v: string.Empty);
            Property(n: "connection", v: string.Empty);
            Property(n: "file", v: string.Empty);
            Property(n: "from", v: string.Empty);
            Property(n: "html", v: true);
            Property(n: "method", v: "get");
            Property(n: "mode", v: "*");
            Property(n: "new-value", v: string.Empty);
            Property(n: "old-value", v: string.Empty);
            Property(n: "subject", v: string.Empty);
            Property(n: "time-out", v: 0);
            Property(n: "to", v: string.Empty);
            Property(n: "url", v: string.Empty);

            Class<TflNameReference>("modes");
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