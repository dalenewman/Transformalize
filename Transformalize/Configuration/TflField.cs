using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflField : CfgNode {

        public TflField() {

            Property(name: "aggregate", value: string.Empty);
            Property(name: "alias", value: string.Empty, required: false, unique: true);
            Property(name: "default", value: string.Empty);
            Property(name: "default-blank", value: false);
            Property(name: "default-empty", value: false);
            Property(name: "default-white-space", value: false);
            Property(name: "delimiter", value: ", ");
            Property(name: "distinct", value: false);
            Property(name: "index", value: short.MaxValue);
            Property(name: "input", value: true);
            Property(name: "label", value: string.Empty);
            Property(name: "length", value: "64");
            Property(name: "name", value: string.Empty, required: true);
            Property(name: "node-type", value: "element");
            Property(name: "optional", value: false);
            Property(name: "output", value: true);
            Property(name: "precision", value: 18);
            Property(name: "primary-key", value: false);
            Property(name: "quoted-with", value: default(char));
            Property(name: "raw", value: false);
            Property(name: "read-inner-xml", value: true);
            Property(name: "scale", value: 9);
            Property(name: "search-type", value: "default");
            Property(name: "sort", value: string.Empty);
            Property(name: "t", value: string.Empty);
            Property(name: "type", value: "string");
            Property(name: "unicode", value: Common.DefaultValue);
            Property(name: "variable-length", value: Common.DefaultValue);

            Collection<TflNameReference>("search-types");
            Collection<TflTransform>("transforms");
        }

    }
}