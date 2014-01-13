using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class XPathOperation : TflOperation {
        private readonly string _outType;
        private readonly string _xPath;
        private readonly XmlReaderSettings _settings = new XmlReaderSettings();

        public XPathOperation(string inKey, string outKey, string outType, string xPath)
            : base(inKey, outKey) {
            _outType = outType;
            _xPath = xPath;
            _settings.ConformanceLevel = ConformanceLevel.Fragment;
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
                }
                yield return row;
            }
        }
    }
}