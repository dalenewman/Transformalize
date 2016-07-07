#region license
// Cfg.Net
// Copyright 2015 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Globalization;
using Cfg.Net.Contracts;

namespace Cfg.Net.Parsers {
    public class JsonNode : INode {
        private Dictionary<string, IAttribute> _attributes;

        public JsonNode() {
            Name = string.Empty;
            Attributes = new List<IAttribute>();
            SubNodes = new List<INode>();
        }

        public JsonNode(string name, Dictionary<string, object> parsed) {
            Name = name;
            Attributes = new List<IAttribute>();
            SubNodes = new List<INode>();
            HandleDictionary(parsed);
        }

        public JsonNode(string name, IList<object> parsed) {
            Name = name;
            Attributes = new List<IAttribute>();
            SubNodes = new List<INode>();
            HandleList(parsed);
        }

        public JsonNode(object parsed) {
            Name = string.Empty;
            Attributes = new List<IAttribute>();
            SubNodes = new List<INode>();

            //dict
            var dict = parsed as Dictionary<string, object>;
            if (dict != null) {
                HandleDictionary(dict);
                return;
            }

            //list
            var list = parsed as List<object>;
            if (list != null) {
                HandleList(list);
            }
        }

        public string Name { get; private set; }
        public List<IAttribute> Attributes { get; private set; }
        public List<INode> SubNodes { get; private set; }

        public bool TryAttribute(string name, out IAttribute attr) {
            if (_attributes == null) {
                _attributes = new Dictionary<string, IAttribute>();
                for (int i = 0; i < Attributes.Count; i++) {
                    _attributes[Attributes[i].Name] = Attributes[i];
                }
            }
            if (_attributes.ContainsKey(name)) {
                attr = _attributes[name];
                return true;
            }
            attr = null;
            return false;
        }

        private void HandleDictionary(Dictionary<string, object> dict) {
            foreach (var pair in dict) {
                ProcessNameAndValue(pair.Key, pair.Value);
            }
        }

        private void ProcessNameAndValue(string name, object value) {
            // objects
            var dict = value as Dictionary<string, object>;
            if (dict != null) {
                SubNodes.Add(new JsonNode(name, dict));
                return;
            }

            // arrays of objects
            var list = value as List<object>;
            if (list != null) {
                SubNodes.Add(new JsonNode(name, list));
                return;
            }

            // attributes
            AddAttribute(Attributes, name, value);
        }

        private static void AddAttribute(ICollection<IAttribute> attributes, string name, object value) {
            attributes.Add(new NodeAttribute { Name = name, Value = value });
        }

        private void HandleList(IList<object> parsed) {
            if (parsed.Count == 0)
                return;

            object first = parsed[0];
            if (first is string || first is long) {
                for (int i = 0; i < parsed.Count; i++) {
                    var dict = new Dictionary<string, object>();
                    dict[i.ToString(CultureInfo.InvariantCulture)] = parsed[i];
                    SubNodes.Add(new JsonNode("add", dict));
                }
            } else {
                for (int i = 0; i < parsed.Count; i++) {
                    ProcessNameAndValue("add", parsed[i]);
                }
            }
        }
    }
}