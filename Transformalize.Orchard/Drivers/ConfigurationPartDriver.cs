using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Drivers {

    public class ConfigurationPartDriver : ContentPartDriver<ConfigurationPart> {

        protected override string Prefix {
            get { return "Transformalize"; }
        }

        //IMPORT, EXPORT
        protected override void Importing(ConfigurationPart part, ImportContentContext context) {
            part.Record.Configuration = context.Attribute(part.PartDefinition.Name, "Configuration");
            part.Record.Modes = context.Attribute(part.PartDefinition.Name, "Modes");
            part.Record.DisplayLog = System.Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "DisplayLog"));
            part.Record.LogLevel = context.Attribute(part.PartDefinition.Name, "LogLevel");
            part.Record.StartAddress = context.Attribute(part.PartDefinition.Name, "StartAddress");
            part.Record.EndAddress = context.Attribute(part.PartDefinition.Name, "EndAddress");
            part.Record.OutputFileExtension = context.Attribute(part.PartDefinition.Name, "OutputFileExtension");
            part.Record.TryCatch = System.Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "TryCatch"));

        }

        protected override void Exporting(ConfigurationPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Configuration", part.Record.Configuration);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Modes", part.Record.Modes);
            context.Element(part.PartDefinition.Name).SetAttributeValue("DisplayLog", part.Record.DisplayLog);
            context.Element(part.PartDefinition.Name).SetAttributeValue("LogLevel", part.Record.LogLevel);
            context.Element(part.PartDefinition.Name).SetAttributeValue("StartAddress", part.Record.StartAddress);
            context.Element(part.PartDefinition.Name).SetAttributeValue("EndAddress", part.Record.EndAddress);
            context.Element(part.PartDefinition.Name).SetAttributeValue("OutputFileExtension", part.Record.OutputFileExtension);
            context.Element(part.PartDefinition.Name).SetAttributeValue("TryCatch", part.Record.TryCatch);
        }

        //GET EDITOR
        protected override DriverResult Editor(ConfigurationPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Configuration_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/Configuration", Model: part, Prefix: Prefix));
        }

        //POST EDITOR
        protected override DriverResult Editor(ConfigurationPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}
