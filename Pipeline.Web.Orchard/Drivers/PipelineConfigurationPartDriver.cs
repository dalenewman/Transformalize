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

using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Drivers {

   public class ConfigurationPartDriver : ContentPartDriver<PipelineConfigurationPart> {

      protected override string Prefix {
         get { return Common.PipelineConfigurationName; }
      }

      //IMPORT, EXPORT
      protected override void Importing(PipelineConfigurationPart part, ImportContentContext context) {
         part.EditorMode = context.Attribute(part.PartDefinition.Name, "EditorMode");
         part.Configuration = context.Attribute(part.PartDefinition.Name, "Configuration");
         part.StartAddress = context.Attribute(part.PartDefinition.Name, "StartAddress");
         part.EndAddress = context.Attribute(part.PartDefinition.Name, "EndAddress");
         part.Runnable = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "Runnable"));
         part.NeedsInputFile = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "NeedsInputFile"));
         part.Modes = context.Attribute(part.PartDefinition.Name, "Modes");
         part.PlaceHolderStyle = context.Attribute(part.PartDefinition.Name, "PlaceHolderStyle");
         part.ClientSideSorting = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "ClientSideSorting"));
         part.MapCircleRadius = Convert.ToInt32(context.Attribute(part.PartDefinition.Name, "MapCircleRadius"));
         part.MapCircleOpacity = Convert.ToDouble(context.Attribute(part.PartDefinition.Name, "MapCircleOpacity"));
         part.PageSizes = context.Attribute(part.PartDefinition.Name, "PageSizes");
         part.MapSizes = context.Attribute(part.PartDefinition.Name, "MapSizes");
         part.EnableInlineParameters = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "EnableInlineParameters"));
         part.ClipTextAt = Convert.ToInt32(context.Attribute(part.PartDefinition.Name, "ClipTextAt"));

         part.CalendarEnabled = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "CalendarEnabled"));
         part.CalendarPaging = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "CalendarPaging"));
         part.CalendarIdField = context.Attribute(part.PartDefinition.Name, "CalendarIdField");
         part.CalendarTitleField = context.Attribute(part.PartDefinition.Name, "CalendarTitleField");
         part.CalendarUrlField = context.Attribute(part.PartDefinition.Name, "CalendarUrlField");
         part.CalendarClassField = context.Attribute(part.PartDefinition.Name, "CalendarClassField");
         part.CalendarStartField = context.Attribute(part.PartDefinition.Name, "CalendarStartField");
         part.CalendarEndField = context.Attribute(part.PartDefinition.Name, "CalendarEndField");

         part.MapEnabled = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "MapEnabled"));
         part.MapPaging = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "MapPaging"));
         part.MapBulkActions = Convert.ToBoolean(context.Attribute(part.PartDefinition.Name, "MapBulkActions"));
         part.MapColorField = context.Attribute(part.PartDefinition.Name, "MapColorField");
         part.MapPopUpField = context.Attribute(part.PartDefinition.Name, "MapPopUpField");
         part.MapLatitudeField = context.Attribute(part.PartDefinition.Name, "MapLatitudeField");
         part.MapLongitudeField = context.Attribute(part.PartDefinition.Name, "MapLongitudeField");
         part.MapZoom = Convert.ToInt32(context.Attribute(part.PartDefinition.Name, "MapZoom"));

         part.ReportRowClassField = context.Attribute(part.PartDefinition.Name, "ReportRowClassField");
         part.ReportRowStyleField = context.Attribute(part.PartDefinition.Name, "ReportRowStyleField");

         part.Migrated = true;
      }

      protected override void Exporting(PipelineConfigurationPart part, ExportContentContext context) {
         if (part.Migrated) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("EditorMode", part.EditorMode);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Configuration", part.Configuration);
            context.Element(part.PartDefinition.Name).SetAttributeValue("StartAddress", part.StartAddress);
            context.Element(part.PartDefinition.Name).SetAttributeValue("EndAddress", part.EndAddress);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Runnable", part.Runnable);
            context.Element(part.PartDefinition.Name).SetAttributeValue("NeedsInputFile", part.NeedsInputFile);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ClientSideSorting", part.ClientSideSorting);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapCircleRadius", part.MapCircleRadius);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapCircleOpacity", part.MapCircleOpacity);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Modes", part.Modes);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PlaceHolderStyle", part.PlaceHolderStyle);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PageSizes", part.PageSizes);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapSizes", part.MapSizes);
            context.Element(part.PartDefinition.Name).SetAttributeValue("EnableInlineParameters", part.EnableInlineParameters);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ClipTextAt", part.ClipTextAt);

            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarEnabled", part.CalendarEnabled);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarPaging", part.CalendarPaging);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarIdField", part.CalendarIdField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarTitleField", part.CalendarTitleField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarUrlField", part.CalendarUrlField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarClassField", part.CalendarClassField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarStartField", part.CalendarStartField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarEndField", part.CalendarEndField);

            context.Element(part.PartDefinition.Name).SetAttributeValue("MapEnabled", part.MapEnabled);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapBulkActions", part.MapBulkActions);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapPaging", part.MapPaging);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapColorField", part.MapColorField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapPopUpField", part.MapPopUpField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapLatitudeField", part.MapLatitudeField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapLongitudeField", part.MapLongitudeField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapZoom", part.MapZoom);

            context.Element(part.PartDefinition.Name).SetAttributeValue("ReportRowClassField", part.ReportRowClassField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ReportRowStyleField", part.ReportRowStyleField);

            context.Element(part.PartDefinition.Name).SetAttributeValue("Migrated", true);
         } else {
            context.Element(part.PartDefinition.Name).SetAttributeValue("EditorMode", part.Record.EditorMode);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Configuration", part.Record.Configuration);
            context.Element(part.PartDefinition.Name).SetAttributeValue("StartAddress", part.Record.StartAddress);
            context.Element(part.PartDefinition.Name).SetAttributeValue("EndAddress", part.Record.EndAddress);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Runnable", part.Record.Runnable);
            context.Element(part.PartDefinition.Name).SetAttributeValue("NeedsInputFile", part.Record.NeedsInputFile);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ClientSideSorting", part.Record.ClientSideSorting);

            context.Element(part.PartDefinition.Name).SetAttributeValue("MapCircleRadius", part.Record.MapCircleRadius);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapCircleOpacity", part.Record.MapCircleOpacity);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Modes", part.Record.Modes);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PlaceHolderStyle", part.Record.PlaceHolderStyle);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PageSizes", part.Record.PageSizes);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapSizes", part.Record.MapSizes);
            context.Element(part.PartDefinition.Name).SetAttributeValue("EnableInlineParameters", part.Record.EnableInlineParameters);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ClipTextAt", part.Record.ClipTextAt);

            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarEnabled", part.Record.CalendarEnabled);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarPaging", part.Record.CalendarPaging);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarIdField", part.Record.CalendarIdField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarTitleField", part.Record.CalendarTitleField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarUrlField", part.Record.CalendarUrlField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarClassField", part.Record.CalendarClassField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarStartField", part.Record.CalendarStartField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CalendarEndField", part.Record.CalendarEndField);

            context.Element(part.PartDefinition.Name).SetAttributeValue("MapEnabled", part.Record.MapEnabled);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapBulkActions", part.Record.MapBulkActions);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapPaging", part.Record.MapPaging);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapColorField", part.Record.MapColorField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapPopUpField", part.Record.MapPopUpField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapLatitudeField", part.Record.MapLatitudeField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapLongitudeField", part.Record.MapLongitudeField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MapZoom", part.Record.MapZoom);

            context.Element(part.PartDefinition.Name).SetAttributeValue("ReportRowClassField", part.Record.ReportRowClassField);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ReportRowStyleField", part.Record.ReportRowStyleField);

            context.Element(part.PartDefinition.Name).SetAttributeValue("Migrated", false);
         }
      }

      //GET EDITOR
      protected override DriverResult Editor(PipelineConfigurationPart part, dynamic shapeHelper) {
         return ContentShape("Parts_" + Prefix + "_Edit", () => shapeHelper
             .EditorTemplate(TemplateName: "Parts/" + Prefix, Model: part, Prefix: Prefix));
      }

      //POST EDITOR
      protected override DriverResult Editor(PipelineConfigurationPart part, IUpdateModel updater, dynamic shapeHelper) {
         updater.TryUpdateModel(part, Prefix, null, null);
         return Editor(part, shapeHelper);
      }

   }
}
