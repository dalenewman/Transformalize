#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Xml;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Xml {
    public class XPathTransform : BaseTransform {

        private readonly Field _input;
        private readonly bool _hasNamespace;
        private readonly bool _xPathIsField;
        private readonly Field _xPathField;

        public XPathTransform(IContext context = null) : base(context, null) {

            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (IsMissing(Context.Operation.Expression)) {
                return;
            }

            if (!string.IsNullOrEmpty(Context.Operation.NameSpace) && string.IsNullOrEmpty(context.Operation.Url)) {
                Error("If you set a namespace, you must also set the url that references the name space.");
            }

            _input = SingleInput();
            _xPathIsField = Context.Process.TryGetField(context.Operation.Expression, out _xPathField);
            _hasNamespace = !string.IsNullOrEmpty(context.Operation.NameSpace);
        }

        public override IRow Operate(IRow row) {

            var xml = (string)row[_input];

            var doc = new XmlDocument();
            var xPath = _xPathIsField ? row[_xPathField].ToString() : Context.Operation.Expression;
            doc.LoadXml(xml);

            XmlNode node;
            if (_hasNamespace) {
                var ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace(Context.Operation.NameSpace, Context.Operation.Url);
                node = doc.SelectSingleNode(xPath, ns);
            } else {
                node = doc.SelectSingleNode(xPath);
            }

            if (node == null) {
                if (Context.Field.Default == Constants.DefaultSetting) {
                    row[Context.Field] = Constants.TypeDefaults()[Context.Field.Type];
                } else {
                    row[Context.Field] = Context.Field.Convert(Context.Field.Default);
                }
            } else {
                var content = Context.Field.ReadInnerXml ? node.InnerXml : node.OuterXml;
                if (string.IsNullOrEmpty(content)) {
                    content = node.InnerText;
                }
                row[Context.Field] = Context.Field.Convert(content);
            }

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("xpath") {
                    Parameters = new List<OperationParameter>(3) {
                        new OperationParameter("expression"),
                        new OperationParameter("name-space",""),
                        new OperationParameter("url", "")
                    }
                }
            };
        }
    }
}