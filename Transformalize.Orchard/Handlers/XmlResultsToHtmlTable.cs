using System.Linq;
using System.Text;
using System.Xml;
using Transformalize.Main;

namespace Transformalize.Orchard.Handlers {

    public class XmlResultsToHtmlTable : IResultsHandler {

        public string Handle(Process[] processes) {
            var xmlBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(xmlBuilder, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            foreach (var process in processes) {
                if (process.Results.Any()) {
                    xmlWriter.WriteStartElement("table");

                    var rows = process.Results.ToList();
                    var columns = rows.First().Columns.Where(c => !c.StartsWith("Tfl")).Select(c => c).ToArray();
                    var fields = process.OutputFields().Where(f => columns.Contains(f.Alias)).Select(f => new[] { f.Alias, string.IsNullOrEmpty(f.Label) ? f.Alias : f.Label, f.Raw.ToString() }).ToArray();

                    xmlWriter.WriteStartElement("thead");
                    xmlWriter.WriteStartElement("tr");
                    if (columns.Any()) {
                        foreach (var field in fields) {
                            xmlWriter.WriteElementString("th", field[1]);
                        }
                    }
                    xmlWriter.WriteEndElement(); //tr
                    xmlWriter.WriteEndElement(); //thead

                    xmlWriter.WriteStartElement("tbody");
                    foreach (var row in process.Results) {
                        xmlWriter.WriteStartElement("tr");
                        foreach (var field in fields) {
                            xmlWriter.WriteStartElement("td");
                            if (field[2].Equals("True")) {
                                xmlWriter.WriteRaw(row[field[0]].ToString());
                            } else {
                                xmlWriter.WriteValue(row[field[0]]);
                            }
                            xmlWriter.WriteEndElement();//td
                        }
                        xmlWriter.WriteEndElement();//tr
                    }
                    xmlWriter.WriteEndElement(); //tbody

                    xmlWriter.WriteEndElement(); //table
                }
            }

            xmlWriter.Flush();
            return xmlBuilder.ToString();
        }
    }
}