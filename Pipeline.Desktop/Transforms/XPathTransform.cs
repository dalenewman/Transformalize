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
using System.Xml;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {
    public class XPathTransform : BaseTransform {

        private readonly Field _input;
        private readonly bool _hasNamespace;
        private readonly bool _xPathIsField;
        private readonly Field _xPathField;

        public XPathTransform(IContext context) : base(context, null) {
            _input = SingleInput();
            _xPathIsField = context.Process.TryGetField(context.Transform.XPath, out _xPathField);
            _hasNamespace = !string.IsNullOrEmpty(context.Transform.NameSpace);
        }

        public override IRow Transform(IRow row) {

            var xml = (string)row[_input];

            var doc = new XmlDocument();
            var xPath = _xPathIsField ? row[_xPathField].ToString() : Context.Transform.XPath;
            doc.LoadXml(xml);

            XmlNode node;
            if (_hasNamespace) {
                var ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace(Context.Transform.NameSpace, Context.Transform.Url);
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
    }
}