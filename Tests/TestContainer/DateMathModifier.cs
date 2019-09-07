#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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

using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;

namespace Tests.TestContainer {
    public class DateMathModifier : ICustomizer {

        private const string DefaultFormat = "yyyy-MM-dd";

        private static void ApplyDateMath(INode node, string name) {
            if (!node.TryAttribute(name, out var valueAttribute) || valueAttribute.Value == null)
                return;

            var value = valueAttribute.Value.ToString();

            if (node.TryAttribute("format", out var formatAttribute) && formatAttribute.Value != null) {
                var format = formatAttribute.Value.ToString();
                valueAttribute.Value = string.IsNullOrEmpty(format) ? DaleNewman.DateMath.Parse(value, DefaultFormat) : DaleNewman.DateMath.Parse(value, format);
            } else {
                valueAttribute.Value = DaleNewman.DateMath.Parse(value, DefaultFormat);
            }

        }

        public void Customize(string parent, INode node, IDictionary<string, string> parameters, ILogger logger) {
            if (parent == "fields" || parent == "calculated-fields") {
                ApplyDateMath(node, "default");
            }
        }

        public void Customize(INode root, IDictionary<string, string> parameters, ILogger logger) {
            var environments = root.SubNodes.FirstOrDefault(n => n.Name == "environments");
            if (environments == null) {
                UpdateParameters(root.SubNodes);
            } else {
                foreach (var environment in environments.SubNodes) {
                    UpdateParameters(environment.SubNodes);
                }
            }
        }

        public void UpdateParameters(List<INode> nodes) {
            var p = nodes.FirstOrDefault(n => n.Name == "parameters");
            if (p == null)
                return;
            foreach (var parameter in p.SubNodes) {
                ApplyDateMath(parameter, "value");
            }
        }
    }
}