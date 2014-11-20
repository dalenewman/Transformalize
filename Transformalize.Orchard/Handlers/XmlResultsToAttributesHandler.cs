using System.Linq;
using System.Text;
using System.Xml;
using Transformalize.Main;

namespace Transformalize.Orchard.Handlers {
    public class XmlResultsToAttributesHandler : IResultsHandler {
        public string Handle(Process[] processes) {
            var xmlBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(xmlBuilder, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            foreach (var rows in processes.Select(p => p.Results)) {
                xmlWriter.WriteStartElement("rows");
                foreach (var row in rows) {
                    xmlWriter.WriteStartElement("row");
                    foreach (var column in row.Columns) {
                        xmlWriter.WriteAttributeString(column, row[column].ToString());
                    }
                    xmlWriter.WriteEndElement(); //row
                }
                xmlWriter.WriteEndElement(); //rows
            }
            xmlWriter.Flush();
            return xmlBuilder.ToString();
        }
    }
}