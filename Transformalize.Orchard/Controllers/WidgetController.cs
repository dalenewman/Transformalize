using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using System.Xml.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class WidgetController : Controller {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransformalizeService _transformalize;
        private readonly IApiService _apiService;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        protected Localizer T { get; set; }
        protected ILogger Logger { get; set; }

        public WidgetController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IApiService apiService
        ) {
            _orchardServices = services;
            _transformalize = transformalize;
            _apiService = apiService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _stopwatch.Start();
        }

        protected static string TransformConfigurationForLoad(string configuration) {
            var xml = XDocument.Parse(configuration).Root;
            if (xml == null) {
                TflLogger.Error(string.Empty, string.Empty, "Unable to transform configuration for load.");
                return configuration;
            }
            ReverseConfiguration(xml);
            return xml.ToString();
        }

        protected static string TransformConfigurationForDownload(string configuration) {
            var xml = XDocument.Parse(configuration).Root;
            if (xml == null) {
                TflLogger.Error(string.Empty, string.Empty, "Unable to transform configuration for download.");
                return configuration;
            }

            ReverseConfiguration(xml);

            var output = xml.Element("processes").Element("add").Element("connections").Elements().First(e => e.Attribute("name").Value.Equals("output", IC));
            output.SetAttributeValue("provider", "file");
            output.SetAttributeValue("file", "@(OutputFile)");
            return xml.ToString();
        }

        protected static void ReverseConfiguration(XContainer xml) {
            // ReSharper disable PossibleNullReferenceException

            var process = xml.Element("processes").Element("add");
            var processName = process.Attribute("name").Value;
            process.SetAttributeValue("star-enabled", "false");

            var connections = process.Element("connections").Elements().ToArray();
            connections.First(c => c.Attribute("name").Value.Equals("output")).SetAttributeValue("name", "temp");
            connections.First(c => c.Attribute("name").Value.Equals("input")).SetAttributeValue("name", "output");
            connections.First(c => c.Attribute("name").Value.Equals("temp")).SetAttributeValue("name", "input");

            var entity = process.Element("entities").Element("add");
            CopyAttributeIfMissing(entity, "name", "alias");
            DefaultAttributeIfMissing(entity, "prepend-process-name-to-output-name", "true");

            entity.SetAttributeValue("name", GetEntityOutputName(entity, processName));

            var fields = entity.Elements("fields").Elements("add").ToArray();

            //if no primary key is set, make all the fields a composite primary key
            if (!fields.Any(f => f.Attribute("primary-key") != null && f.Attribute("primary-key").Value.Equals("true", IC))) {
                foreach (var field in fields) {
                    field.SetAttributeValue("primary-key", "true");
                }
            }

            // These fields will be used alot in clients.  Make sure they exist with default values.
            CopyAttributesIfMissing(fields, "name", "alias");
            CopyAttributesIfMissing(fields, "alias", "label");
            DefaultAttributesIfMissing(fields, "input", "true");
            DefaultAttributesIfMissing(fields, "output", "true");
            DefaultAttributesIfMissing(fields, "type", "string");

            SwapAttributes(fields, "name", "alias");

            entity.SetAttributeValue("detect-changes", "false");
            if (entity.Attributes("delete").Any() && entity.Attribute("delete").Value.Equals("true", IC)) {
                entity.Attributes("delete").Remove();

                var filter = new XElement("filter");
                var add = new XElement("add");
                add.SetAttributeValue("left", "TflDeleted");
                add.SetAttributeValue("right", "0");
                filter.Add(add);
                entity.Add(filter);

                var lastField = fields.Last();
                var deleted = new XElement("add");
                deleted.SetAttributeValue("name", "TflDeleted");
                deleted.SetAttributeValue("type", "boolean");
                deleted.SetAttributeValue("output", "false");
                deleted.SetAttributeValue("label", "Deleted");
                lastField.AddAfterSelf(deleted);
            }

            // ReSharper restore PossibleNullReferenceException
        }

        protected static string GetEntityOutputName(XElement element, string processName) {
            var tflEntity = new Entity() {
                Alias = element.Attribute("alias").Value,
                Name = element.Attribute("name").Value,
                PrependProcessNameToOutputName =
                    Convert.ToBoolean(element.Attribute("prepend-process-name-to-output-name").Value.ToLower())
            };
            return Common.EntityOutputName(tflEntity, processName);
        }

        protected static void CopyAttributesIfMissing(IEnumerable<XElement> elements, string from, string to) {
            foreach (var element in elements) {
                CopyAttributeIfMissing(element, from, to);
            }
        }

        protected static void CopyAttributeIfMissing(XElement element, string from, string to) {
            if (element.Attribute(to) == null) {
                element.SetAttributeValue(to, element.Attribute(from).Value);
            }
        }

        protected static void SwapAttributes(IEnumerable<XElement> elements, string left, string right) {
            foreach (var element in elements) {
                SwapAttribute(element, left, right);
            }
        }

        protected static void SwapAttribute(XElement element, string left, string right) {
            var leftValue = element.Attribute(left) == null ? string.Empty : element.Attribute(left).Value;
            var rightValue = element.Attribute(right) == null ? string.Empty : element.Attribute(right).Value;

            element.SetAttributeValue(left, rightValue);
            element.SetAttributeValue(right, leftValue);
        }

        protected static void DefaultAttributesIfMissing(IEnumerable<XElement> elements, string attribute, string value) {
            foreach (var element in elements) {
                DefaultAttributeIfMissing(element, attribute, value);
            }
        }

        protected static void DefaultAttributeIfMissing(XElement element, string attribute, string value) {
            if (element.Attribute(attribute) == null) {
                element.SetAttributeValue(attribute, value);
            }
        }

        protected ApiResponse Run(
            ApiRequest request,
            ConfigurationPart part,
            NameValueCollection query,
            string modifiedConfiguration = null
        ) {

            var errorNumber = 500;
            try {
                var tflRequest = new TransformalizeRequest(part) {
                    Configuration = modifiedConfiguration ?? part.Configuration,
                    Options = new Options(),
                    Query = query
                };

                errorNumber++;
                var tflResponse = _transformalize.Run(tflRequest);

                errorNumber++;
                return new ApiResponse(
                    request,
                    tflRequest.Configuration,
                    tflResponse
                );
            } catch (Exception ex) {
                request.Status = errorNumber;
                request.Message = ex.Message;
                return new ApiResponse(
                    request,
                    part.Configuration,
                    new TransformalizeResponse()
                );
            }
        }

    }
}