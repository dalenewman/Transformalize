using System.IO;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Main;

namespace Transformalize.Orchard.Handlers {

    public class JsonResultsToObjectHandler : IResultsHandler {

        public string Handle(Process[] processes) {
            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            foreach (var process in processes) {
                writer.WriteStartArray();
                foreach (var row in process.Results) {
                    writer.WriteStartObject();
                    foreach (var alias in process.OutputFields().Aliases()) {
                        writer.WritePropertyName(alias);
                        writer.WriteValue(row[alias]);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            writer.Flush();
            return sw.ToString();
        }
    }
}