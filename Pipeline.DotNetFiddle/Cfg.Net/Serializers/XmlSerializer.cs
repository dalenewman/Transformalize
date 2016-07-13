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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cfg.Net.Contracts;

namespace Cfg.Net.Serializers {
    public sealed class XmlSerializer : ISerializer {
        public string Serialize(CfgNode node) {
            return InnerSerialize(node);
        }

        string InnerSerialize(CfgNode node) {

            var type = node.GetType();
            var attribute = type.GetCustomAttributes(typeof(CfgAttribute), true).FirstOrDefault() as CfgAttribute;
            if (attribute != null && !attribute.serialize)
                return string.Empty;

            var name = !string.IsNullOrEmpty(attribute?.name) ? attribute.name : type.Name;
            var meta = CfgMetadataCache.GetMetadata(type);
            var builder = new StringBuilder();

            if (JustAttributes(meta, node)) {
                builder.Append("<add");
                SerializeAttributes(meta, node, builder);
                builder.Append(" />");
            } else {
                builder.Append("<");
                builder.Append(name);
                SerializeAttributes(meta, node, builder);
                builder.AppendLine(">");
                SerializeElements(meta, node, builder, 1);
                builder.Append("</");
                builder.Append(name);
                builder.Append(">");
            }

            return builder.ToString();
        }

        static bool JustAttributes(IDictionary<string, CfgMetadata> meta, object node) {
            if (meta.All(kv => kv.Value.ListType == null))
                return true;

            foreach (var pair in meta.Where(kv => kv.Value.ListType != null)) {
                var list = (IList)meta[pair.Key].Getter(node);
                if (list != null && list.Count > 0)
                    return false;
            }
            return true;
        }

        void SerializeElements(IDictionary<string, CfgMetadata> meta, object node, StringBuilder builder, int level) {

            foreach (var pair in meta.Where(kv => kv.Value.ListType != null)) {

                var m = meta[pair.Key];

                if (!m.Attribute.serialize)
                    continue;

                var items = (IList)m.Getter(node);
                if (items == null || items.Count == 0)
                    continue;
                var name = m.Attribute.name;

                Indent(builder, level);
                builder.Append("<");
                builder.Append(name);
                builder.AppendLine(">");

                foreach (var item in items) {
                    var metaData = CfgMetadataCache.GetMetadata(item.GetType());
                    Indent(builder, level + 1);
                    builder.Append("<add");
                    SerializeAttributes(metaData, item, builder);

                    if (JustAttributes(metaData, item)) {
                        builder.AppendLine(" />");
                    } else {
                        builder.AppendLine(">");
                        SerializeElements(metaData, item, builder, level + 2);
                        Indent(builder, level + 1);
                        builder.AppendLine("</add>");
                    }
                }

                Indent(builder, level);
                builder.Append("</");
                builder.Append(name);
                builder.AppendLine(">");
            }

        }

        private static void Indent(StringBuilder builder, int level) {
            for (var i = 0; i < level * 4; i++) {
                builder.Append(' ');
            }
        }

        private void SerializeAttributes(IDictionary<string, CfgMetadata> meta, object obj, StringBuilder builder) {
            if (meta.Count > 0) {
                foreach (var pair in meta.Where(kv => kv.Value.ListType == null)) {

                    if (!pair.Value.Attribute.serialize)
                        continue;

                    var value = pair.Value.Getter(obj);
                    if (value == null || value.Equals(pair.Value.Attribute.value) || (!pair.Value.Attribute.ValueIsSet && pair.Value.Default != null && pair.Value.Default.Equals(value))) {
                        continue;
                    }

                    var name = meta[pair.Key].Attribute.name;

                    builder.Append(" ");
                    builder.Append(name);
                    builder.Append("=\"");

                    var stringValue = pair.Value.PropertyInfo.PropertyType == typeof(string) ? (string)value : value.ToString();
                    if (pair.Value.PropertyInfo.PropertyType == typeof(bool)) {
                        stringValue = stringValue.ToLower();
                    }
                    builder.Append(Encode(stringValue));
                    builder.Append("\"");
                }
            } else {
                var dict = obj as IProperties;
                if (dict == null)
                    return;
                foreach (var pair in dict) {
                    builder.Append(" ");
                    builder.Append(pair.Key);
                    builder.Append("=\"");
                    builder.Append(pair.Value == null ? string.Empty : Encode(pair.Value.ToString()));
                    builder.Append("\"");
                }
            }
        }

        public string Encode(string value) {
            var builder = new StringBuilder();
            for (var i = 0; i < value.Length; i++) {
                var ch = value[i];
                if (ch <= '>') {
                    switch (ch) {
                        case '<':
                            builder.Append("&lt;");
                            break;
                        case '>':
                            builder.Append("&gt;");
                            break;
                        case '"':
                            builder.Append("&quot;");
                            break;
                        case '\'':
                            builder.Append("&#39;");
                            break;
                        case '&':
                            builder.Append("&amp;");
                            break;
                        default:
                            builder.Append(ch);
                            break;
                    }
                } else {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }


    }
}