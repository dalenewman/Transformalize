using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Drivers {

    public class FilePartDriver : ContentPartDriver<FilePart> {

        protected override string Prefix {
            get { return "Transformalize"; }
        }

        //IMPORT, EXPORT
        protected override void Importing(FilePart part, ImportContentContext context) {
            part.Record.FullPath = context.Attribute(part.PartDefinition.Name, "FullPath");
            part.Record.Direction = context.Attribute(part.PartDefinition.Name, "Direction");
        }

        protected override void Exporting(FilePart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("FullPath", part.Record.FullPath);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Direction", part.Record.Direction);
        }

        //GET EDITOR
        protected override DriverResult Editor(FilePart part, dynamic shapeHelper) {
            return ContentShape("Parts_File_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/File", Model: part, Prefix: Prefix));
        }

        //POST EDITOR
        protected override DriverResult Editor(FilePart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}
