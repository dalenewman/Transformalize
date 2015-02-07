using System.IO;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Main;

namespace Transformalize.Orchard.Handlers {
    public class JsonResultsToDictionaryHandler : IResultsHandler {
        public string Handle(Process[] processes) {
            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            foreach (var process in processes) {
                writer.WriteStartArray();
                foreach (var row in process.Results) {
                    writer.WriteStartArray();
                    foreach (var alias in process.OutputFields().Aliases()) {
                        writer.WriteStartObject();
                        writer.WritePropertyName("key");
                        writer.WriteValue(alias);
                        writer.WritePropertyName("value");
                        writer.WriteValue(row[alias]);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.Flush();
            return sw.ToString();
        }
    }
}