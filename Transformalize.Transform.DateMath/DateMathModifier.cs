using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Transformalize.Transform.DateMath {
    public class DateMathModifier : ICustomizer {

        private const string DefaultFormat = "yyyy-MM-dd";

        private static void ApplyDateMath(INode node, string name) {
            IAttribute valueAttribute;
            if (!node.TryAttribute(name, out valueAttribute) || valueAttribute.Value == null)
                return;

            var value = valueAttribute.Value.ToString();

            IAttribute formatAttribute;
            if (node.TryAttribute("format", out formatAttribute) && formatAttribute.Value != null) {
                var format = formatAttribute.Value.ToString();
                valueAttribute.Value = string.IsNullOrEmpty(format) ? DaleNewman.DateMath.Parse(value, DefaultFormat) : DaleNewman.DateMath.Parse(value, format);
            } else {
                valueAttribute.Value = DaleNewman.DateMath.Parse(value, DefaultFormat);
            }
        }

        public void Customize(string parent, INode node, IDictionary<string, string> parameters, ILogger logger) {
            if (parent == "parameters") {
                ApplyDateMath(node, "value");
            }

            if (parent == "fields" || parent == "calculated-fields") {
                ApplyDateMath(node, "default");
            }
        }

        public void Customize(INode root, IDictionary<string, string> parameters, ILogger logger) { }
    }
}