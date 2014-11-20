using System.Linq;
using System.Text;
using System.Xml.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Main;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers {
    public static class JsonContentHandler {
        private const string JSON_TEMPLATE = @"{{
    ""request"":""{0}"",
    ""status"":{1},
    ""message"":""{2}"",
    ""time"":{3},
    ""environments"":{4},
    ""processes"":{5},
    ""response"":{6}
}}";

        public static string GetContent(ApiRequest request, Process[] proc, string meta) {

            var builder = new StringBuilder();
            var converter = new OneWayXmlNodeConverter();
            var doc = XDocument.Parse(request.Configuration);
            var environments = JsonConvert.SerializeObject(doc.Descendants("environments").Any() ? doc.Descendants("environments").First().Nodes() : null, Formatting.None, converter);
            var processes = JsonConvert.SerializeObject(doc.Descendants("processes").First().Nodes(), Formatting.None, converter);

            switch (request.RequestType) {
                case ApiRequestType.MetaData:
                    var metaData = JsonConvert.SerializeObject(XDocument.Parse(meta).Descendants("entities").First(), Formatting.None, converter);
                    builder.AppendFormat(JSON_TEMPLATE, "metadata", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, metaData);
                    return builder.ToString();

                case ApiRequestType.Configuration:
                    builder.AppendFormat(JSON_TEMPLATE, "configuration", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, "null");
                    return builder.ToString();

                case ApiRequestType.Execute:
                    string results;
                    switch (request.Flavor) {
                        case "arrays":
                            results = new JsonResultsToArrayHandler().Handle(proc);
                            break;
                        default:
                            results = new JsonResultsToDictionaryHandler().Handle(proc);
                            break;
                    }
                    builder.AppendFormat(JSON_TEMPLATE, "execute", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, results);
                    return builder.ToString();

                default:
                    builder.AppendFormat(JSON_TEMPLATE, "configuration", 200, "OK", request.Stopwatch.ElapsedMilliseconds, environments, processes, "null");
                    return builder.ToString();
            }
        }

    }
}