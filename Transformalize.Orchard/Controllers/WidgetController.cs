using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Cfg.Net.Ext;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Transformalize.Configuration;
using Transformalize.Main;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class WidgetController : Controller {

        private readonly ITransformalizeService _transformalize;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        protected Localizer T { get; set; }
        protected ILogger Logger { get; set; }

        public WidgetController(
            ITransformalizeService transformalize
        ) {
            _transformalize = transformalize;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _stopwatch.Start();
        }

        protected static void TransformConfigurationForLoad(TflRoot root) {
            ReverseConfiguration(root);
        }

        protected static void TransformConfigurationForDownload(TflRoot root, Dictionary<string, string> query) {

            ReverseConfiguration(root);

            var output = root.Processes[0].Connections.First(c => c.Name == "output");
            output.Provider = "file";
            output.File = query.ContainsKey("OutputFile") ? query["OutputFile"] : string.Empty;

        }

        protected static void ReverseConfiguration(TflRoot root) {

            var process = root.Processes[0];
            process.StarEnabled = false;

            //swap input and output
            process.Connections.First(c => c.Name.Equals("output")).Name = "temp";
            process.Connections.First(c => c.Name.Equals("input")).Name = "output";
            process.Connections.First(c => c.Name.Equals("temp")).Name = "input";

            var entity = process.Entities[0];
            entity.Name = GetEntityOutputName(entity, process.Name);

            //if no primary key is set, make all the fields a composite primary key
            if (!entity.Fields.Any(f => f.PrimaryKey)) {
                foreach (var field in entity.Fields) {
                    field.PrimaryKey = true;
                }
            }

            //aliases are used to write to output, so swap when reversing
            foreach (var field in entity.Fields) {
                var temp = field.Name;
                field.Name = field.Alias;
                field.Alias = temp;
            }

            entity.DetectChanges = false;

            if (!entity.Delete)
                return;

            entity.Delete = false;
            entity.Filter.Add(new TflFilter {
                Left = "TflDeleted",
                Right = "0"
            }.WithDefaults());

            entity.Fields.Add(new TflField {
                Name = "TflDeleted",
                Type = "bool",
                Output = false,
                Label = "Deleted"
            }.WithDefaults());
        }

        protected static string GetEntityOutputName(TflEntity entity, string processName) {
            var tflEntity = new Entity {
                Alias = entity.Alias,
                Name = entity.Name,
                PrependProcessNameToOutputName = entity.PrependProcessNameToOutputName
            };
            return Common.EntityOutputName(tflEntity, processName);
        }

        protected ApiResponse Run(
            ApiRequest request,
            TransformalizeRequest transformalizeRequest
        ) {

            var errorNumber = 500;
            try {
                errorNumber++;
                var tflResponse = _transformalize.Run(transformalizeRequest);

                errorNumber++;
                return new ApiResponse(
                    request,
                    transformalizeRequest.Configuration,
                    tflResponse
                );
            } catch (Exception ex) {
                request.Status = errorNumber;
                request.Message = ex.Message;
                return new ApiResponse(
                    request,
                    transformalizeRequest.Configuration,
                    new TransformalizeResponse()
                );
            }
        }

    }
}