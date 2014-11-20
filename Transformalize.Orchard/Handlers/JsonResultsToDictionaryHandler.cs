using System.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Main;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers
{
    public class JsonResultsToDictionaryHandler : IResultsHandler {
        public string Handle(Process[] processes) {
            return JsonConvert.SerializeObject(processes.Select(p => p.Results), Formatting.None);
        }
    }
}