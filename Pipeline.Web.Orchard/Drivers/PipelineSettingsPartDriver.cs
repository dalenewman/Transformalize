using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Drivers {

    public class PipelineSettingsPartDriver : ContentPartDriver<PipelineSettingsPart> {

        private const string TemplateName = "Parts/PipelineSettings";

        public PipelineSettingsPartDriver() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override void Importing(PipelineSettingsPart part, ImportContentContext context) {
            part.Record.EditorTheme = context.Attribute(part.PartDefinition.Name, "EditorTheme");
            part.Record.Shorthand = context.Attribute(part.PartDefinition.Name, "Shorthand");
        }

        protected override void Exporting(PipelineSettingsPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("EditorTheme", part.Record.EditorTheme);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Shorthand", part.Record.Shorthand);
        }

        protected override string Prefix { get { return "PipelineSettings"; } }

        // GET EDITOR
        protected override DriverResult Editor(PipelineSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_PipelineSettings_Edit",
                    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix))
                    .OnGroup("Pipeline.NET");
        }

        // POST EDITOR
        protected override DriverResult Editor(PipelineSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Parts_PipelineSettings_Edit", () => {
                updater.TryUpdateModel(part, Prefix, null, null);
                return shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix);
            }).OnGroup("Pipeline.NET");
        }
    }
}