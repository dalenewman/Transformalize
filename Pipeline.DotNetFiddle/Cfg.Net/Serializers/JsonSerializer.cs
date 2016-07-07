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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cfg.Net.Contracts;
using Cfg.Net.Ext;

namespace Cfg.Net.Serializers {
    public class JsonSerializer : ISerializer {
        public string Serialize(CfgNode node) {
            return InnerSerialize(node);
        }

        private string InnerSerialize(CfgNode node) {
            var meta = CfgMetadataCache.GetMetadata(node.GetType());
            var builder = new StringBuilder();
            if (meta.All(kv => kv.Value.ListType == null)) {
                builder.Append("{");
                SerializeAttributes(meta, node, builder);
                builder.TrimEnd(", ");
                builder.Append(" }");
            } else {
                builder.AppendLine("{");
                SerializeAttributes(meta, node, builder);
                SerializeElements(meta, node, builder, 1);
                builder.TrimEnd(", ");
                builder.AppendLine();
                builder.Append("}");
            }

            return builder.ToString();
        }

        private void SerializeElements(IDictionary<string, CfgMetadata> meta, object node, StringBuilder sb, int level) {

            var pairs = meta.Where(kv => kv.Value.ListType != null && kv.Value.Attribute.serialize).ToArray();

            for (var y = 0; y < pairs.Length; y++) {

                var pair = pairs[y];
                var nodes = (IList)meta[pair.Key].Getter(node);
                if (nodes == null || nodes.Count == 0)
                    continue;

                Indent(sb, level);
                sb.Append("\"");
                sb.Append(meta[pair.Key].Attribute.name);
                sb.AppendLine("\":[");

                var count = nodes.Count;
                var last = count - 1;
                for (var i = 0; i < count; i++) {
                    var item = nodes[i];
                    var metaData = CfgMetadataCache.GetMetadata(item.GetType());
                    Indent(sb, level + 1);
                    sb.Append("{");
                    SerializeAttributes(metaData, item, sb);
                    if (metaData.Any(kv => kv.Value.ListType != null)) {
                        SerializeElements(metaData, item, sb, level + 2);
                        Indent(sb, level + 1);
                        Next(sb, i, last);
                    } else {
                        Next(sb, i, last);
                    }
                }

                Indent(sb, level);
                sb.Append("]");

                if (y < pairs.Length - 1) {
                    sb.Append(",");
                }

            }

        }

        private static void Next(StringBuilder sb, int i, int last) {
            if (i < last) {
                sb.TrimEnd(", ");
                sb.Append(" }");
                sb.AppendLine(",");
            } else {
                sb.TrimEnd(", ");
                sb.AppendLine(" }");
            }
        }

        private static void Indent(StringBuilder builder, int level) {
            for (var i = 0; i < level * 4; i++) {
                builder.Append(' ');
            }
        }

        private static void SerializeAttributes(IDictionary<string, CfgMetadata> meta, object obj, StringBuilder sb) {

            if (meta.Count > 0) {
                var pairs = meta.Where(kv => kv.Value.ListType == null).ToArray();
                for (var i = 0; i < pairs.Length; i++) {
                    var pair = pairs[i];
                    if (!pair.Value.Attribute.serialize) {
                        continue;
                    }
                    var value = pair.Value.Getter(obj);
                    if (value == null || value.Equals(pair.Value.Attribute.value) || (!pair.Value.Attribute.ValueIsSet && pair.Value.Default != null && pair.Value.Default.Equals(value))) {
                        continue;
                    }

                    var type = pair.Value.PropertyInfo.PropertyType;

                    sb.Append(" \"");
                    sb.Append(meta[pair.Key].Attribute.name);
                    sb.Append("\":");
                    sb.Append(ValueToString(type, value));
                    sb.Append(",");
                }

            } else {
                var dict = obj as IProperties;
                if (dict == null) return;

                foreach (var pair in dict) {
                    sb.Append(" \"");
                    sb.Append(pair.Key);
                    sb.Append("\":");
                    sb.Append(TypeToString[pair.Value.GetType()](pair.Value));
                    sb.Append(",");
                }
            }
        }

        public static object Encode(object value) {
            if (value == null)
                return "null";

            var str = value as string;

            if (str == null) {
                return value;
            }

            var len = str.Length;

            if (len == 0) {
                return "\"\"";
            }

            int i;
            var sb = new StringBuilder(len + 6);

            sb.Append('"');

            for (i = 0; i < len; i += 1) {
                var c = str[i];
                switch (c) {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ') {
                            var t = "000" + BytesToHexString(new[] { Convert.ToByte(c) });
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        } else {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append('"');

            return sb.ToString();
        }

        private static string BytesToHexString(byte[] bytes) {
            var c = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++) {
                var b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        private static readonly Dictionary<Type, Func<object, string>> TypeToString = new Dictionary<Type, Func<object, string>> {
            {typeof(string), v => Encode((string)v).ToString() },
            {typeof(bool), v=> v.ToString().ToLower()},
            {typeof(DateTime), v => "\"" + ((DateTime)v).ToString("o") + "\""},
            {typeof(Guid), v=> "\"" + ((Guid)v) + "\"" },
            {typeof(int), v=> v.ToString() },
            {typeof(short), v=> v.ToString() },
            {typeof(long), v=> v.ToString() },
            {typeof(double), v=> v.ToString() },
            {typeof(float), v=> v.ToString() },
            {typeof(decimal), v=> v.ToString() },
            {typeof(char), v=> "\"" + Encode(v.ToString()) + "\""}
        };

        private static string ValueToString(Type type, object value) {
            return TypeToString.ContainsKey(type) ? TypeToString[type](value) : "\"" + value + "\"";
        }
    }
}