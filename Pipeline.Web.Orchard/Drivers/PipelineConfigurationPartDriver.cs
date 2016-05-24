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

    public class ConfigurationPartDriver : ContentPartDriver<PipelineConfigurationPart> {

        protected override string Prefix {
            get { return "PipelineConfiguration"; }
        }

        //IMPORT, EXPORT
        protected override void Importing(PipelineConfigurationPart part, ImportContentContext context) {
            part.Record.Configuration = context.Attribute(part.PartDefinition.Name, "Configuration");
            part.Record.StartAddress = context.Attribute(part.PartDefinition.Name, "StartAddress");
            part.Record.EndAddress = context.Attribute(part.PartDefinition.Name, "EndAddress");
        }

        protected override void Exporting(PipelineConfigurationPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Configuration", part.Record.Configuration);
            context.Element(part.PartDefinition.Name).SetAttributeValue("StartAddress", part.Record.StartAddress);
            context.Element(part.PartDefinition.Name).SetAttributeValue("EndAddress", part.Record.EndAddress);
        }
        
        //GET EDITOR
        protected override DriverResult Editor(PipelineConfigurationPart part, dynamic shapeHelper) {
            return ContentShape("Parts_PipelineConfiguration_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/PipelineConfiguration", Model: part, Prefix: Prefix));
        }

        //POST EDITOR
        protected override DriverResult Editor(PipelineConfigurationPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}
