using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Transformalize.Main;
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
</transformalize>";

        private static string XmlNodesToString(IEnumerable<XNode> nodes) {
            return string.Concat(nodes.Select(n => n.ToString()));
        }

        public static string GetContent(ApiRequest request, Process[] proc, string meta) {
            var builder = new StringBuilder();
            var doc = XDocument.Parse(request.Configuration);
            var environments = doc.Descendants("environments").Any() ? XmlNodesToString(doc.Descendants("environments").First().Nodes()) : string.Empty;
            var processes = XmlNodesToString(doc.Descendants("processes").First().Nodes());

            switch (request.RequestType) {
                case ApiRequestType.MetaData:
                    var metaData = XDocument.Parse(meta).Descendants("entities").First().ToString();
                    builder.AppendFormat(XML_TEMPLATE, "metadata", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, metaData);
                    return builder.ToString();

                case ApiRequestType.Configuration:
                    builder.AppendFormat(XML_TEMPLATE, "configuration", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, "null");
                    return builder.ToString();

                case ApiRequestType.Execute:
                    string results;
                    switch (request.Flavor) {
                        case "attributes":
                            results = new XmlResultsToAttributesHandler().Handle(proc);
                            break;
                        case "table":
                            results = new XmlResultsToHtmlTable().Handle(proc);
                            break;
                        default:
                            results = new XmlResultsToDictionaryHandler().Handle(proc);
                            break;
                    }
                    builder.AppendFormat(XML_TEMPLATE, "execute", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, results);
                    return builder.ToString();

                default:
                    builder.AppendFormat(XML_TEMPLATE, "configuration", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, "null");
                    return builder.ToString();
            }

        }
    }
}