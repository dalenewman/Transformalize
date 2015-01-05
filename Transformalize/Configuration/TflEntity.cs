using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflEntity : CfgNode {

        public TflEntity() {

            Property(name: "alias", value: string.Empty, required: false, unique: true);
            Property(name: "connection", value: "input");
            Property(name: "delete", value: false);
            Property(name: "detect-changes", value: true);
            Property(name: "group", value: false);
            Property(name: "name", value: string.Empty, required: true);
            Property(name: "no-lock", value: false);
            Property(name: "pipeline-threading", value: "Default");
            Property(name: "prefix", value: string.Empty);
            Property(name: "prepend-process-name-to-output-name", value: true);
            Property(name: "sample", value: 100);
            Property(name: "schema", value: string.Empty);
            Property(name: "query-keys", value: string.Empty);
            Property(name: "query", value: string.Empty);
            Property(name: "script-keys", value: string.Empty);
            Property(name: "script", value: string.Empty);
            Property(name: "trim-all", value: false);
            Property(name: "unicode", value: string.Empty);
            Property(name: "variable-length", value: string.Empty);
            Property(name: "version", value: string.Empty);

            Collection<TflFilter>("filter");
            Collection<TflField>("fields");
            Collection<TflCalculatedField>("calculated-fields");
            Collection<TflIo>("input");
            Collection<TflIo>("output");
        }

    }
}