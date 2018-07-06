using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Controllers {

    [Themed]
    public class WidgetController : Controller {

        private readonly Stopwatch _stopwatch = new Stopwatch();

        protected Localizer T { get; set; }
        protected ILogger Logger { get; set; }

        public WidgetController(
        ) {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _stopwatch.Start();
        }

        protected static void TransformConfigurationForLoad(Transformalize.Configuration.Process root) {
            ReverseConfiguration(root);
        }

        protected static void TransformConfigurationForDownload(Transformalize.Configuration.Process root, Dictionary<string, string> query) {

            ReverseConfiguration(root);

            var output = root.Connections.First(c => c.Name == "output");
            output.Provider = "file";
            output.File = query.ContainsKey("OutputFile") ? query["OutputFile"] : string.Empty;

        }

        protected static void ReverseConfiguration(Transformalize.Configuration.Process process) {

            //swap input and output
            process.Connections.First(c => c.Name.Equals("output")).Name = "temp";
            process.Connections.First(c => c.Name.Equals("input")).Name = "output";
            process.Connections.First(c => c.Name.Equals("temp")).Name = "input";

            var entity = process.Entities[0];
            entity.Name = entity.OutputViewName(process.Name);

            //if no primary key is set, make all the fields a composite primary key
            if (!entity.Fields.Any(f => f.PrimaryKey)) {
                foreach (var field in entity.Fields) {
                    field.PrimaryKey = true;
                }
            }

            //aliases are used to write to output, so swap when reversing
            foreach (var field in entity.Fields.Where(f=>!f.System)) {
                var temp = field.Name;
                field.Name = field.Alias;
                field.Alias = temp;
            }

            entity.Update = false;
            entity.Delete = false;

            entity.Filter.Add(new Transformalize.Configuration.Filter {
                Expression = "TflDeleted = 0"
            });

       }

        protected static string GetEntityOutputName(Entity entity, string processName) {
            var tflEntity = new Entity {
                Alias = entity.Alias,
                Name = entity.Name,
                PrependProcessNameToOutputName = entity.PrependProcessNameToOutputName
            };
            return tflEntity.OutputTableName(processName);
        }



    }
}