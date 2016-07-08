using System;
using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Cfg.Net.Environment {

    /// <summary>
    /// Updatea the parameters dictionary from a collection of elements with attributes name and value.
    /// </summary>
    public class ParameterModifier : IRootModifier {
        private readonly string _nameAttribute;
        private readonly string _valueAttribute;

        public ParameterModifier() : this("name", "value") { }

        public ParameterModifier(string nameAttribute, string valueAttribute) {
            _nameAttribute = nameAttribute;
            _valueAttribute = valueAttribute;
        }

        public void Modify(INode root, IDictionary<string, string> parameters) {

            foreach (var parameter in root.SubNodes) {
                string name = null;
                object value = null;
                foreach (var attribute in parameter.Attributes) {
                    if (attribute.Name == _nameAttribute) {
                        name = attribute.Value.ToString();
                    } else if (attribute.Name == _valueAttribute) {
                        value = attribute.Value;
                    }
                }
                if (name != null && value != null) {
                    if (parameters.ContainsKey(name)) {
                        IAttribute attr;
                        if (parameter.TryAttribute(_valueAttribute, out attr)) {
                            attr.Value = parameters[name];
                        }
                    } else {
                        parameters[name] = value.ToString();
                    }
                }
            }
        }
    }
}