using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class XPathOperation : ShouldRunOperation {
        private readonly string _outType;
        private readonly string _xPath;
        private readonly XmlReaderSettings _settings = new XmlReaderSettings();

        public XPathOperation(string inKey, string outKey, string outType, string xPath)
            : base(inKey, outKey) {
            _outType = outType;
            _xPath = xPath;
            _settings.ConformanceLevel = ConformanceLevel.Fragment;
            Name = string.Format("XPathOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var target = string.Empty;
                    var reader = new StringReader(row[InKey].ToString());
                    using (var xmlReader = XmlReader.Create(reader, _settings)) {
                        var navigator = new XPathDocument(xmlReader).CreateNavigator();
                        var result = navigator.Select(_xPath);
                        while (result.MoveNext()) {
                            target += result.Current.Value;
                        }
                    }
                    row[OutKey] = Common.ConversionMap[_outType](target);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}