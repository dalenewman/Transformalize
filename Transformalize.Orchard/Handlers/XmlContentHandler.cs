using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers {
    public static class XmlContentHandler {

        private const string XML_TEMPLATE = @"<transformalize>
    <request>{0}</request>
    <status>{1}</status>
    <message>{2}</message>
    <time>{3}</time>
    <environments>{4}</environments>
    <processes>{5}</processes>
    <response>{6}</response>
    <log>{7}</log>
</transformalize>";

        private static string XmlNodesToString(IEnumerable<XNode> nodes) {
            return string.Concat(nodes.Select(n => n.ToString()));
        }

        public static string LogsToXml(IEnumerable<string> logs) {
            var xmlBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(xmlBuilder, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
            foreach (var log in logs) {
                xmlWriter.WriteStartElement("entry");
                var attributes = log.Split(new[] { " | " }, 5, StringSplitOptions.None);

                xmlWriter.WriteAttributeString("time", attributes[0]);
                xmlWriter.WriteAttributeString("level", attributes[1].TrimEnd());
                xmlWriter.WriteAttributeString("process", attributes[2]);
                xmlWriter.WriteAttributeString("entity", attributes[3]);
                xmlWriter.WriteAttributeString("message", attributes[4].TrimEnd());

                xmlWriter.WriteEndElement();
            }
            xmlWriter.Flush();
            return xmlBuilder.ToString();
        }

        public static string GetContent(ApiRequest request, TransformalizeResponse response, string meta) {

            if (request.Status != 200) {
                return ApiContentHandler.GetErrorContent(XML_TEMPLATE, request);
            }

            var builder = new StringBuilder();
            var doc = XDocument.Parse(request.Configuration);
            var environments = doc.Descendants("environments").Any() ? XmlNodesToString(doc.Descendants("environments").First().Nodes()) : string.Empty;
            var processes = XmlNodesToString(doc.Descendants("processes").First().Nodes());

            switch (request.RequestType) {
                case ApiRequestType.MetaData:
                    var metaData = XDocument.Parse(meta).Descendants("entities").First().ToString();
                    builder.AppendFormat(XML_TEMPLATE, "metadata", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, metaData, LogsToXml(response.Log));
                    return builder.ToString();

                case ApiRequestType.Configuration:
                    builder.AppendFormat(XML_TEMPLATE, "configuration", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, string.Empty, LogsToXml(response.Log));
                    return builder.ToString();

                case ApiRequestType.Execute:
                    string results;
                    switch (request.Flavor) {
                        case "attributes":
                            results = new XmlResultsToAttributesHandler().Handle(response.Processes);
                            break;
                        case "table":
                            results = new XmlResultsToHtmlTable().Handle(response.Processes);
                            break;
                        default:
                            results = new XmlResultsToDictionaryHandler().Handle(response.Processes);
                            break;
                    }
                    builder.AppendFormat(XML_TEMPLATE, "execute", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, results, LogsToXml(response.Log));
                    return builder.ToString();

                default:
                    builder.AppendFormat(XML_TEMPLATE, "configuration", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, string.Empty, LogsToXml(response.Log));
                    return builder.ToString();
            }

        }
    }
}