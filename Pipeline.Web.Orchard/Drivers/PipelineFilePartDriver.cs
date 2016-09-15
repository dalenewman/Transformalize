#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Drivers {

   public class PipelineFilePartDriver : ContentPartDriver<PipelineFilePart> {

        protected override string Prefix => Common.PipelineFileName;

       //IMPORT, EXPORT
        protected override void Importing(PipelineFilePart part, ImportContentContext context) {
            part.Record.FullPath = context.Attribute(part.PartDefinition.Name, "FullPath");
            part.Record.Direction = context.Attribute(part.PartDefinition.Name, "Direction");
        }

        protected override void Exporting(PipelineFilePart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("FullPath", part.Record.FullPath);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Direction", part.Record.Direction);
        }

        //GET EDITOR
        protected override DriverResult Editor(PipelineFilePart part, dynamic shapeHelper) {
            return ContentShape("Parts_" + Prefix + "_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/" + Prefix, Model: part, Prefix: Prefix));
        }

        //POST EDITOR
        protected override DriverResult Editor(PipelineFilePart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
        
        // FOR ADMIN LIST RESULTS
        protected override DriverResult Display(PipelineFilePart part, string displayType, dynamic shapeHelper) {
            if (displayType.StartsWith("Summary")) {
                return Combined(
                    ContentShape("Parts_"+Prefix+"_SummaryAdmin", () => shapeHelper.Parts_PipelineFile_SummaryAdmin(Part: part))
                );
            }
            return null;
        }

    }
}
