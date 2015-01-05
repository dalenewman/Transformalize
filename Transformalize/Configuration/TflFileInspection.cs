using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflFileInspection : CfgNode {

        public TflFileInspection() {

            Property(name: "name", value: string.Empty, required: true, unique: true);
            Property(name: "sample", value: 100);
            Property(name: "max-length", value: 0);
            Property(name: "min-length", value: 0);

            Collection<TflType>("types");
            Collection<TflDelimiter>("delimiters", true);
        }

    }
}