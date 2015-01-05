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

    }
}