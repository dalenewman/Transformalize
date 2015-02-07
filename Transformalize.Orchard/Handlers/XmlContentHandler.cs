using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Transformalize.Extensions;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers {
    public static class XmlContentHandler {
        private const string MINIMAL_RESPONSE_TEMPLATE = @"<response><add request=""{0}"" status=""{1}"" message=""{2}"" time=""{3}""/></response>";
        private const string RESPONSE_TEMPLATE = @"<response>
    <add request=""{0}"" status=""{1}"" message=""{2}"" time=""{3}"">
        <rows>{4}</rows>
        <content>{6}</content>
        <log>{5}</log>
    </add>
</response>
        ";
        private const string XML_TEMPLATE = @"<tfl>
    <response>
        <add request=""{0}"" status=""{1}"" message=""{2}"" time=""{3}"">
            <rows>{6}</rows>
            <content>{8}</content>
            <log>{7}</log>
        </add>
    </response>
    <environments>{4}</environments>
    <processes>{5}</processes>
</tfl>";

        private static string XmlNodesToString(IEnumerable<XNode> nodes) {
            return string.Concat(nodes.Select(n => n.ToString()));
        }

        public static string LogsToXml(IEnumerable<string> logs) {
            var xmlBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(xmlBuilder, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            foreach (var log in logs) {
                xmlWriter.WriteStartElement("add");
                var attributes = log.Split(new[] { " | " }, 5, StringSplitOptions.None);

                xmlWriter.WriteAttributeString("time", attributes[0]);
                xmlWriter.WriteAttributeString("level", attributes[1].TrimEnd());
                xmlWriter.WriteAttributeString("process", attributes[2]);
                xmlWriter.WriteAttributeString("entity", attributes[3]);
                xmlWriter.WriteAttributeString("message", attributes[4].TrimEnd(new[] { ' ', '\r', '\n' }));

                xmlWriter.WriteEndElement();
            }
            xmlWriter.Flush();
            return xmlBuilder.ToString();
        }

        public static string GetContent(ApiRequest request, string configuration, TransformalizeResponse response) {

            var content = string.Empty;
            var results = string.Empty;
            var builder = new StringBuilder();

            switch (request.RequestType) {
                case ApiRequestType.MetaData:
                    builder.Append(configuration);
                    builder.InsertFormat(builder.LastIndexOf('<'), RESPONSE_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds, string.Empty, LogsToXml(response.Log));
                    return builder.ToString();

                case ApiRequestType.Configuration:
                    builder.Append(configuration);
                    builder.InsertFormat(builder.LastIndexOf('<'), MINIMAL_RESPONSE_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds);
                    return builder.ToString();

                case ApiRequestType.Execute:

                    var doc = XDocument.Parse(configuration).Root;
                    var nodes = doc.Element("processes");
                    nodes.Descendants("connections").Remove();
                    nodes.Descendants("parameters").Remove();
                    var processes = XmlNodesToString(nodes.Nodes());

                    switch (request.Flavor) {
                        case "attributes":
                            results = new XmlResultsToAttributesHandler().Handle(response.Processes);
                            break;
                        case "table":
                            content = new XmlResultsToHtmlTable().Handle(response.Processes);
                            break;
                        default:
                            results = new XmlResultsToDictionaryHandler().Handle(response.Processes);
                            break;
                    }
                    builder.AppendFormat(XML_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds, string.Empty, processes, results, LogsToXml(response.Log), content);
                    return builder.ToString();

                default:
                    if (request.Status == 200) {
                        request.Status = 400;
                        request.Message = "Bad Request";
                    }
                    builder.AppendFormat(XML_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds, string.Empty, string.Empty, results, LogsToXml(response.Log), content);
                    return builder.ToString();
            }

        }
    }
}