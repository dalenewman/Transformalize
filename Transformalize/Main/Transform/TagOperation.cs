using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Web;
using System.Xml;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {

    public class TagOperation2 : ShouldRunOperation {
        private readonly string _tag;
        private readonly IParameters _parameters;
        private readonly int _length;
        private readonly MemoryStream _memory = new MemoryStream();
        private readonly XmlWriter _xmlWriter;

        public TagOperation2(string outKey, string tag, IParameters parameters)
            : base(string.Empty, outKey) {
            _tag = tag;
            _parameters = parameters;
            _length = _parameters.Count;
            var xmlWriterSettings = new XmlWriterSettings {
                Encoding = new UTF8Encoding(false),
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = false
            };
            _xmlWriter = XmlWriter.Create(_memory, xmlWriterSettings);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    _xmlWriter.WriteStartElement(_tag);
                    string content = null;
                    for (var i = 0; i < _length; i++) {
                        var name = _parameters[i].Name;
                        var value = HttpUtility.HtmlDecode((row[name] ?? _parameters[i].Value).ToString());
                        if (name.Equals("content")) {
                            content = value;
                        } else {
                            _xmlWriter.WriteAttributeString(name, value);
                        }
                    }
                    if (content == null) {
                        _xmlWriter.WriteEndElement();
                    } else {
                        _xmlWriter.WriteString(content);
                        _xmlWriter.WriteEndElement();
                    }
                    _xmlWriter.Flush();
                    row[OutKey] = Encoding.UTF8.GetString(_memory.ToArray());
                    _memory.SetLength(0);
                } else {
                    Skip();
                }
                yield return row;
            }

        }
    }

    public class TagOperation : ShouldRunOperation {
        private readonly string _tag;
        private readonly IParameters _parameters;
        private readonly bool _decode;
        private readonly bool _encode;

        public TagOperation(string outKey, string tag, IParameters parameters, bool decode, bool encode)
            : base(string.Empty, outKey) {
            _tag = tag;
            _parameters = parameters;
            _decode = decode;
            _encode = encode;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var sb = StringBuilders.GetObject();
                    sb.Append('<');
                    sb.Append(_tag);
                    string content = null;
                    foreach (var p in _parameters) {
                        var value = (row[p.Key] ?? (p.Value.ValueReferencesField ? row[p.Value.Value] ?? "parameter name conflict!" : p.Value.Value)).ToString();
                        if (_decode) {
                            value = HttpUtility.HtmlDecode(value);
                        }
                        if (p.Value.Name.Equals("content")) {
                            content = value;
                        } else {
                            sb.Append(' ');
                            sb.Append(p.Value.Name);
                            sb.Append("=\"");
                            if (_encode) {
                                XmlEncodeOperation.SanitizeXmlString(HttpUtility.HtmlAttributeEncode(value), ref sb);
                            } else {
                                XmlEncodeOperation.SanitizeXmlString(value, ref sb);
                            }
                            sb.Append('\"');
                        }
                    }
                    if (content == null) {
                        sb.Append(" />");
                    } else {
                        sb.Append('>');
                        if (_encode) {
                            XmlEncodeOperation.SanitizeXmlString(HttpUtility.HtmlEncode(content), ref sb);
                        } else {
                            XmlEncodeOperation.SanitizeXmlString(content, ref sb);
                        }
                        sb.Append("</");
                        sb.Append(_tag);
                        sb.Append(">");
                    }
                    row[OutKey] = sb.ToString();
                    sb.Clear();
                    StringBuilders.PutObject(sb);
                } else {
                    Skip();
                }
                yield return row;
            }
        }
    }
}