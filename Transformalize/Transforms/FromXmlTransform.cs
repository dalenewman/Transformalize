#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Linq;
using Cfg.Net.Parsers.nanoXML;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    /// <summary>
    /// The default (portable) fromxml transform, based on Cfg-Net's modified copy of NanoXml.
    /// </summary>
    public class FromXmlTransform : BaseTransform {
        private const StringComparison Ic = StringComparison.OrdinalIgnoreCase;

        private readonly Field _input;
        private readonly Dictionary<string, Field> _attributes = new Dictionary<string, Field>();
        private readonly Dictionary<string, Field> _elements = new Dictionary<string, Field>();
        private readonly bool _searchAttributes;
        private readonly int _total;

        public FromXmlTransform(IContext context)
            : base(context, null) {

            if (!context.Operation.Parameters.Any()) {
                Error($"The {context.Operation.Method} transform requires a collection of output fields.");
                Run = false;
                return;
            }

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

        public override IRow Operate(IRow row) {
            var xml = row[_input] as string;
            if (string.IsNullOrEmpty(xml)) {
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
                            try {
                                row[field] = field.Convert(value);
                            } catch (Exception ex) {
                                Context.Error(ex.Message);
                                Context.Error(value);
                            }
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