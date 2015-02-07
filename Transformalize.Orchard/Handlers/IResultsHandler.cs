using System.Linq;
using System.Text;
using System.Xml;
using Transformalize.Main;

namespace Transformalize.Orchard.Handlers {
    public interface IResultsHandler {
        string Handle(Process[] processes);
    }

    public class XmlResultsToDictionaryHandler : IResultsHandler {

        public string Handle(Process[] processes) {

            var xmlBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(xmlBuilder, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            foreach (var rows in processes.Select(p => p.Results)) {
                foreach (var row in rows) {
                    xmlWriter.WriteStartElement("add");
                    xmlWriter.WriteStartElement("pairs");
                    foreach (var column in row.Columns) {
                        xmlWriter.WriteStartElement("add");
                        xmlWriter.WriteAttributeString("key", column);
                        xmlWriter.WriteAttributeString("value", row[column].ToString());
                        xmlWriter.WriteEndElement(); //add
                    }
                    xmlWriter.WriteEndElement(); //pairs
                    xmlWriter.WriteEndElement(); //add
                }
            }
            xmlWriter.Flush();
            return xmlBuilder.ToString();
        }
    }
}