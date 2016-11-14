#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using System.IO;
using System.Linq;
using System.Xml;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {

    /// <summary>
    /// Converted from Transformalize v1alpha
    /// </summary>
    public class FromXmlTransform : BaseTransform {
        private readonly IRowFactory _rowFactory;

        private readonly string _root;
        private readonly bool _findRoot;

        private const StringComparison Ic = StringComparison.OrdinalIgnoreCase;
        private readonly bool _searchAttributes;
        private readonly Dictionary<string, Field> _nameMap = new Dictionary<string, Field>();
        private static readonly XmlReaderSettings Settings = new XmlReaderSettings {
            IgnoreWhitespace = true,
            IgnoreComments = true
        };

        private readonly Field _input;
        private readonly Field[] _fields;
        private readonly Field[] _outerFields;
        private readonly Dictionary<string, object> _typeDefaults;

        public FromXmlTransform(IContext context, IRowFactory rowFactory) : base(context, null) {

            _rowFactory = rowFactory;
            _input = SingleInputForMultipleOutput();
            var output = MultipleOutput();
            _fields = context.GetAllEntityFields().ToArray();
            _outerFields = _fields.Except(output).ToArray();
            _typeDefaults = Constants.TypeDefaults();

            _root = context.Transform.Root;
            _findRoot = !string.IsNullOrEmpty(context.Transform.Root);

            foreach (var field in output) {
                if (!_searchAttributes && field.NodeType.Equals("attribute", Ic)) {
                    _searchAttributes = true;
                }
                _nameMap[field.Name] = field;
            }
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            foreach (var row in rows) {
                var outerRow = row;
                var innerRow = _rowFactory.Create();
                foreach (var field in _fields) {
                    innerRow[field] = field.Default == Constants.DefaultSetting ? _typeDefaults[field.Type] : field.Convert(field.Default);
                }

                var innerRows = new List<IRow>();
                string startKey = null;

                var xml = row[_input] as string;

                if (!string.IsNullOrEmpty(xml)) {
                    xml = xml.Trim();
                    using (var reader = XmlReader.Create(new StringReader(xml), Settings)) {

                        if (_findRoot) {
                            do {
                                reader.Read();
                            } while (reader.Name != _root);
                        } else {
                            reader.Read();
                        }

                        do {
                            if (_nameMap.ContainsKey(reader.Name)) {

                                // must while here because reader.Read*Xml advances the reader
                                while (_nameMap.ContainsKey(reader.Name) && reader.IsStartElement()) {
                                    InnerRow(ref startKey, reader.Name, ref innerRow, ref outerRow, ref innerRows);

                                    var field = _nameMap[reader.Name];
                                    var value = field.ReadInnerXml ? reader.ReadInnerXml() : reader.ReadOuterXml();
                                    if (value != string.Empty)
                                        innerRow[field] = field.Convert(value);
                                }

                            } else if (_searchAttributes && reader.HasAttributes) {
                                for (var i = 0; i < reader.AttributeCount; i++) {
                                    reader.MoveToNextAttribute();
                                    if (!_nameMap.ContainsKey(reader.Name))
                                        continue;

                                    InnerRow(ref startKey, reader.Name, ref innerRow, ref outerRow, ref innerRows);

                                    var field = _nameMap[reader.Name];
                                    if (!string.IsNullOrEmpty(reader.Value)) {
                                        innerRow[field] = field.Convert(reader.Value);
                                    }
                                }
                            }
                            if (_findRoot && !reader.IsStartElement() && reader.Name == _root) {
                                break;
                            }
                        } while (reader.Read());
                    }
                }
                AddInnerRow(ref innerRow, ref outerRow, ref innerRows);
                foreach (var r in innerRows) {
                    yield return r;
                }
            }
        }

        private static bool ShouldYieldRow(ref string startKey, string key) {
            if (startKey == null) {
                startKey = key;
            } else if (startKey.Equals(key)) {
                return true;
            }
            return false;
        }

        private void InnerRow(ref string startKey, string key, ref IRow innerRow, ref IRow outerRow, ref List<IRow> innerRows) {
            if (!ShouldYieldRow(ref startKey, key))
                return;

            AddInnerRow(ref innerRow, ref outerRow, ref innerRows);
        }

        private void AddInnerRow(ref IRow innerRow, ref IRow outerRow, ref List<IRow> innerRows) {
            var r = _rowFactory.Clone(innerRow, _fields);

            foreach (var field in _outerFields) {
                r[field] = outerRow[field];
            }

            innerRows.Add(r);
            innerRow = _rowFactory.Create();
            foreach (var field in _fields) {
                innerRow[field] = field.Default == Constants.DefaultSetting ? _typeDefaults[field.Type] : field.Convert(field.Default);
            }
        }

        public override IRow Transform(IRow row) {
            throw new NotImplementedException();
        }
    }

}
