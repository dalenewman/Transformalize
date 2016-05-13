#region license
// Transformalize
// Copyright 2013 Dale Newman
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
using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Parsers.nanoXML;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    /// <summary>
    /// The default (portable) fromxml transform, based on Cfg-Net's modified copy of NanoXml.
    /// </summary>
    public class FromXmlTransform : BaseTransform, ITransform {

        const StringComparison Ic = StringComparison.OrdinalIgnoreCase;

        readonly Field _input;
        readonly Dictionary<string, Field> _attributes = new Dictionary<string, Field>();
        readonly Dictionary<string, Field> _elements = new Dictionary<string, Field>();
        readonly bool _searchAttributes;
        readonly int _total;

        public FromXmlTransform(IContext context)
            : base(context) {
            _input = SingleInputForMultipleOutput();
            var output = MultipleOutput();

            foreach (var f in output) {
                if (f.NodeType.Equals("attribute", Ic)) {
                    _attributes[f.Name] = f;
                } else {
                    _elements[f.Name] = f;
                }
            }

            _searchAttributes = _attributes.Count > 0;
            _total = _elements.Count + _attributes.Count;

        }

        public IRow Transform(IRow row) {
            var xml = row.GetString(_input);
            if (xml.Equals(string.Empty)) {
                Increment();
                return row;
            }

            var count = 0;
            var doc = new NanoXmlDocument(xml);
            if (_elements.ContainsKey(doc.RootNode.Name)) {
                var field = _elements[doc.RootNode.Name];
                row[field] = field.Convert(doc.RootNode.Value ?? (field.ReadInnerXml ? doc.RootNode.InnerText() : doc.RootNode.ToString()));
                count++;
            }

            var subNodes = doc.RootNode.SubNodes.ToArray();
            while (subNodes.Any()) {
                var nextNodes = new List<NanoXmlNode>();
                foreach (var node in subNodes) {
                    if (_elements.ContainsKey(node.Name)) {
                        var field = _elements[node.Name];
                        count++;
                        var value = node.Value ?? (field.ReadInnerXml ? node.InnerText() : node.ToString());
                        if (!string.IsNullOrEmpty(value)) {
                            row[field] = field.Convert(value);
                        }
                    }
                    if (_searchAttributes) {
                        foreach (var attribute in node.Attributes.Where(attribute => _attributes.ContainsKey(attribute.Name))) {
                            var field = _attributes[attribute.Name];
                            count++;
                            if (!string.IsNullOrEmpty(attribute.Value)) {
                                row[field] = field.Convert(attribute.Value);
                            }
                        }
                    }
                    if (count < _total) {
                        nextNodes.AddRange(node.SubNodes);
                    }
                }
                subNodes = nextNodes.ToArray();
            }
            Increment();
            return row;
        }

    }
}