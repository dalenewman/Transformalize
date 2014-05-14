using System;
using System.Xml.Linq;

namespace Transformalize.Libs.SolrNet.Utils {
    public static class X {
        public static string ValueOrNull(this XAttribute attr) {
            return attr == null ? null : attr.Value;
        }

        public static Func<XElement, bool> AttrEq(string name, string value) {
            return e => e.Attribute(name).ValueOrNull() == value;
        }
    }
}
