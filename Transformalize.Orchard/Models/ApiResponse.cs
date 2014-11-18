using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Main;
using Formatting = Transformalize.Libs.Newtonsoft.Json.Formatting;

namespace Transformalize.Orchard.Models {

    [Serializable]
    public class ApiResponse {

        private const string XML_TEMPLATE = @"<transformalize>
    <request>{0}</request>
    <status>{1}</status>
    <message>{2}</message>
    <time>{3}</time>
    <environments>{4}</environments>
    <processes>{5}</processes>
    <response>{6}</response>
</transformalize>";

        private const string JSON_TEMPLATE = @"{{
    ""request"":""{0}"",
    ""status"":{1},
    ""message"":""{2}"",
    ""time"":{3},
    ""environments"":{4},
    ""processes"":{5},
    ""response"":{6}
}}";

        private readonly ApiRequest _request;
        private readonly Process[] _processes = new Process[0];
        private readonly string _metaData;

        public int Status { get; set; }
        public string Message { get; set; }

        public ApiResponse(ApiRequest request) {
            _request = request;
            Status = 200;
            Message = "OK";
        }

        public ApiResponse(ApiRequest request, string metaData) {
            _request = request;
            _metaData = metaData;
            Status = 200;
            Message = "OK";
        }

        public ApiResponse(ApiRequest request, Process[] processes) {
            _processes = processes;
            _request = request;
            Status = 200;
            Message = "OK";
        }

        private string JsonContent() {

            var builder = new StringBuilder();
            var converter = new OneWayXmlNodeConverter();
            var doc = XDocument.Parse(_request.Configuration);
            var environments = JsonConvert.SerializeObject(doc.Descendants("environments").Any() ? doc.Descendants("environments").First().Nodes() : null, Formatting.None, converter);
            var processes = JsonConvert.SerializeObject(doc.Descendants("processes").First().Nodes(), Formatting.None, converter);

            switch (_request.RequestType) {
                case ApiRequestType.MetaData:
                    var metaData = JsonConvert.SerializeObject(XDocument.Parse(_metaData).Descendants("entities").First(), Formatting.None, converter);
                    builder.AppendFormat(JSON_TEMPLATE, "metadata", Status, Message, _request.Stopwatch.ElapsedMilliseconds, environments, processes, metaData);
                    return builder.ToString();

                case ApiRequestType.Configuration:
                    builder.AppendFormat(JSON_TEMPLATE, "configuration", Status, Message, _request.Stopwatch.ElapsedMilliseconds, environments, processes, "null");
                    return builder.ToString();

                case ApiRequestType.Execute:
                    var results = JsonConvert.SerializeObject(_processes.Select(p => p.Results), Formatting.None);
                    builder.AppendFormat(JSON_TEMPLATE, "execute", Status, Message, _request.Stopwatch.ElapsedMilliseconds, environments, processes, results);
                    return builder.ToString();

                default:
                    builder.AppendFormat(JSON_TEMPLATE, "configuration", Status, Message, _request.Stopwatch.ElapsedMilliseconds, environments, processes, "null");
                    return builder.ToString();
            }
        }

        private static string XmlNodesToString(IEnumerable<XNode> nodes) {
            return string.Concat(nodes.Select(n => n.ToString()));
        }

        private string ResultsToXmlAttributes() {
            var xmlBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(xmlBuilder, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            foreach (var rows in _processes.Select(p => p.Results)) {
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

        private string ResultsToHtmlTable() {
            var xmlBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(xmlBuilder, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            foreach (var process in _processes) {
                if (process.Results.Any()) {
                    xmlWriter.WriteStartElement("table");

                    var rows = process.Results.ToList();
                    var columns = rows.First().Columns.Where(c => !c.StartsWith("Tfl")).Select(c=>c).ToArray();
                    var fields = process.OutputFields().Where(f => columns.Contains(f.Alias)).Select(f => new [] {f.Alias, string.IsNullOrEmpty(f.Label) ? f.Alias : f.Label, f.Raw.ToString()}).ToArray();

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

        private string ResultsToXmlDictionary() {
            var xmlBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(xmlBuilder, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            foreach (var rows in _processes.Select(p => p.Results)) {
                xmlWriter.WriteStartElement("rows");
                foreach (var row in rows) {
                    xmlWriter.WriteStartElement("row");
                    foreach (var column in row.Columns) {
                        xmlWriter.WriteStartElement("item");
                        xmlWriter.WriteAttributeString("key", column);
                        xmlWriter.WriteAttributeString("value", row[column].ToString());
                        xmlWriter.WriteEndElement(); //item
                    }
                    xmlWriter.WriteEndElement(); //row
                }
                xmlWriter.WriteEndElement(); //rows
            }
            xmlWriter.Flush();
            return xmlBuilder.ToString();
        }

        private string XmlContent() {
            var builder = new StringBuilder();
            var doc = XDocument.Parse(_request.Configuration);
            var environments = doc.Descendants("environments").Any() ? XmlNodesToString(doc.Descendants("environments").First().Nodes()) : string.Empty;
            var processes = XmlNodesToString(doc.Descendants("processes").First().Nodes());

            switch (_request.RequestType) {
                case ApiRequestType.MetaData:
                    var metaData = XDocument.Parse(_metaData).Descendants("entities").First().ToString();
                    builder.AppendFormat(XML_TEMPLATE, "metadata", Status, Message, _request.Stopwatch.ElapsedMilliseconds, environments, processes, metaData);
                    return builder.ToString();

                case ApiRequestType.Configuration:
                    _request.Stopwatch.Stop();
                    builder.AppendFormat(XML_TEMPLATE, "configuration", Status, Message, _request.Stopwatch.ElapsedMilliseconds, environments, processes, "null");
                    return builder.ToString();

                case ApiRequestType.Execute:
                    string results;
                    switch (_request.Query["flavor"]) {
                        case "attributes":
                            results = ResultsToXmlAttributes();
                            break;
                        case "table":
                            results = ResultsToHtmlTable();
                            break;
                        default:
                            results = ResultsToXmlDictionary();
                            break;
                    }
                    builder.AppendFormat(XML_TEMPLATE, "execute", Status, Message, _request.Stopwatch.ElapsedMilliseconds, environments, processes, results);
                    return builder.ToString();

                default:
                    builder.AppendFormat(XML_TEMPLATE, "configuration", Status, Message, _request.Stopwatch.ElapsedMilliseconds, environments, processes, "null");
                    return builder.ToString();
            }
        }


        public ContentResult ContentResult(string format, string flavor = null) {
            return new ContentResult() {
                Content = Content(),
                ContentType = MimeType(format)
            };
        }

        private string Content() {
            switch (_request.Query["format"]) {
                case "table":
                    return "<table></table>";
                case "xml":
                    return XmlContent();
                default:
                    return JsonContent();
            }
        }

        private static string MimeType(string format) {
            switch (format.ToLower()) {
                case "table":
                    return "text/html";
                case "xml":
                    return "text/xml";
                default:
                    return "application/json";
            }
        }

    }
}