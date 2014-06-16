using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class XmlEncodeOperation : ShouldRunOperation {
        public XmlEncodeOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = string.Format("XmlEncodeOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = SanitizeXmlString(HttpUtility.HtmlEncode(row[InKey]));
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }

        public string SanitizeXmlString(string xml) {
            var buffer = new StringBuilder(xml.Length);
            foreach (var c in xml.Where(c => IsLegalXmlChar(c))) {
                buffer.Append(c);
            }
            return buffer.ToString();
        }

        public bool IsLegalXmlChar(int character) {
            return (
                character == 0x9 /* == '\t' == 9   */          ||
                character == 0xA /* == '\n' == 10  */          ||
                character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
                );
        }
    }
}