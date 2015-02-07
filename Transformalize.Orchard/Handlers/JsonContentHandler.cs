using System;
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

        public static string LogsToJson(IEnumerable<string> logs) {
            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            writer.WriteStartArray();
            foreach (var log in logs) {
                writer.WriteStartObject();

                var attributes = log.Split(new[] { " | " }, 5, StringSplitOptions.None);
                writer.WritePropertyName("time");
                writer.WriteValue(attributes[0]);
                writer.WritePropertyName("level");
                writer.WriteValue(attributes[1].TrimEnd());
                writer.WritePropertyName("process");
                writer.WriteValue(attributes[2]);
                writer.WritePropertyName("entity");
                writer.WriteValue(attributes[3]);
                writer.WritePropertyName("message");
                writer.WriteValue(attributes[4].TrimEnd(new[] { ' ', '\r', '\n' }));

                writer.WriteEndObject();
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
            var content = "{}";

            var tfl = new TflRoot(configuration, null);
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