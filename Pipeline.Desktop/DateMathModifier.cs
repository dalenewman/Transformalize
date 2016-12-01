using System;
using System.Collections.Generic;
using Cfg.Net.Contracts;
using dalenewman;

namespace Pipeline.Desktop {
    public class DateMathModifier : IRootModifier {

        private const string DefaultFormat = "yyyy-MM-dd";

        public void Modify(INode root, IDictionary<string, string> parameters) {
            TraverseNodes(root.SubNodes);
        }

        private static void TraverseNodes(IEnumerable<INode> nodes) {
            foreach (var node in nodes) {
                if (node.Attributes.Count > 0) {
                    ApplyDateMath(node, "value");
                    ApplyDateMath(node, "default");
                }
                TraverseNodes(node.SubNodes);
            }
        }

        private static void ApplyDateMath(INode node, string name) {
            IAttribute valueAttribute;
            if (!node.TryAttribute(name, out valueAttribute) || valueAttribute.Value == null)
                return;

            var value = valueAttribute.Value.ToString();

            IAttribute formatAttribute;
            if (node.TryAttribute("format", out formatAttribute) && formatAttribute.Value != null) {
                var format = formatAttribute.Value.ToString();
                valueAttribute.Value = string.IsNullOrEmpty(format) ? DateMath.Parse(value, DefaultFormat) : DateMath.Parse(value, format);
            } else {
                valueAttribute.Value = DateMath.Parse(value, DefaultFormat);
            }
        }

    }
}