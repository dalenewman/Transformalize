using System.Collections.Generic;
using System.IO;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Serialization;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers {
    public static class JsonContentHandler {
        private const string JSON_TEMPLATE = @"{{
    ""environments"":{4},
    ""processes"":{5},
    ""response"": [{{
        ""request"":""{0}"",
        ""status"":{1},
        ""message"":""{2}"",
        ""time"":{3},
        ""rows"":{6},
        ""content"":{8},
        ""log"":{7}
    }}]
}}";

        public static string LogsToJson(IEnumerable<string[]> logs) {
            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            writer.WriteStartArray();
            foreach (var log in logs) {
                if (log.Length <= 4)
                    continue;
                writer.WriteStartObject(); //add
                writer.WritePropertyName("time");
                writer.WriteValue(log[0]);
                writer.WritePropertyName("level");
                writer.WriteValue(log[1].TrimEnd());
                writer.WritePropertyName("process");
                writer.WriteValue(log[2]);
                writer.WritePropertyName("entity");
                writer.WriteValue(log[3]);
                writer.WritePropertyName("message");
                writer.WriteValue(log[4].TrimEnd(new[] { ' ', '\r', '\n' }));
                writer.WriteEndObject(); //add
            }
            writer.WriteEndArray();
            writer.Flush();
            return sw.ToString();
        }

        public static string GetContent(ApiRequest request, string configuration, TransformalizeResponse response) {

            var builder = new StringBuilder();
            var processes = "[]";
            var environments = "[]";
            var results = "[]";
            const string content = "{}";

            var tfl = new TflRoot(configuration);
            var settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            switch (request.RequestType) {

                case ApiRequestType.MetaData:
                    processes = JsonConvert.SerializeObject(tfl.Processes, Formatting.None, settings);
                    builder.AppendFormat(JSON_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds, environments, processes, results, LogsToJson(response.Log), content);
                    return builder.ToString();

                case ApiRequestType.Configuration:
                    processes = JsonConvert.SerializeObject(tfl.Processes, Formatting.None, settings);
                    environments = JsonConvert.SerializeObject(tfl.Environments, Formatting.None, settings);
                    builder.AppendFormat(JSON_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds, environments, processes, results, LogsToJson(response.Log), content);
                    return builder.ToString();

                case ApiRequestType.Enqueue:
                    builder.AppendFormat(JSON_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds, environments, processes, results, LogsToJson(response.Log), content);
                    return builder.ToString();

                case ApiRequestType.Execute:
                    foreach (var process in tfl.Processes) {
                        process.Connections = new List<TflConnection>();
                    }
                    processes = JsonConvert.SerializeObject(tfl.Processes, Formatting.None, settings);
                    switch (request.Flavor) {
                        case "arrays":
                            results = new JsonResultsToArrayHandler().Handle(response.Processes);
                            break;
                        case "array":
                            goto case "arrays";
                        case "dictionaries":
                            results = new JsonResultsToDictionaryHandler().Handle(response.Processes);
                            break;
                        case "dictionary":
                            goto case "dictionaries";
                        default:
                            results = new JsonResultsToObjectHandler().Handle(response.Processes);
                            break;
                    }
                    builder.AppendFormat(JSON_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds, environments, processes, results, LogsToJson(response.Log), content);
                    return builder.ToString();

                default:
                    if (request.Status == 200) {
                        request.Status = 400;
                        request.Message = "Bad Request";
                    }
                    builder.AppendFormat(JSON_TEMPLATE, request.RequestType, request.Status, request.Message, request.Stopwatch.ElapsedMilliseconds, environments, processes, results, LogsToJson(response.Log), content);
                    return builder.ToString();
            }
        }

    }
}