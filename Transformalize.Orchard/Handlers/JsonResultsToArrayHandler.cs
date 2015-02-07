using System.IO;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Main;

namespace Transformalize.Orchard.Handlers {

    public class JsonResultsToArrayHandler : IResultsHandler {

        public string Handle(Process[] processes) {
            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            foreach (var process in processes) {
                writer.WriteStartArray();
                foreach (var row in process.Results) {
                    writer.WriteStartArray();
                    foreach (var alias in process.OutputFields().Aliases()) {
                        writer.WriteValue(row[alias]);
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