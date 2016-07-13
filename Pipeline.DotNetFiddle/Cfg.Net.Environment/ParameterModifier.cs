#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
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