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
using System.Data;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard {

   public class CoreMigrations : DataMigrationImpl {
      public ILogger Logger { get; set; }
      public Localizer T { get; set; }
      private readonly IOrchardServices _orchardServices;
      private readonly INotifier _notifier;

      public CoreMigrations(IOrchardServices orchardServices, INotifier notifier) {
         Logger = NullLogger.Instance;
         T = NullLocalizer.Instance;
         _orchardServices = orchardServices;
         _notifier = notifier;
      }

      public int Create() {

         try {

            SchemaBuilder.CreateTable(Common.PipelineConfigurationName + "PartRecord", table => table
                .ContentPartRecord()
                .Column("Configuration", DbType.String, column => column.Unlimited())
                .Column("StartAddress", DbType.String)
                .Column("EndAddress", DbType.String)
            );

            ContentDefinitionManager.StoreTypeDefinition(
                new ContentTypeDefinition(Common.PipelineConfigurationName, "Pipeline")
            );

            ContentDefinitionManager.AlterTypeDefinition(Common.PipelineConfigurationName, cfg => cfg
                .Creatable()
                .WithSetting("Description", "Transformalize Pipeline")
                .WithPart(Common.PipelineConfigurationName + "Part")
                .WithPart("CommonPart")
                .WithPart("TitlePart")
                .WithPart("IdentityPart")
                .WithPart("ContentPermissionsPart", builder => builder
                    .WithSetting("ContentPermissionsPartSettings.View", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.Publish", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.Edit", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.Delete", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.Preview", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.ViewOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.PublishOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.EditOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.DeleteOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.PreviewOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.DisplayedRoles", "Authenticated,Anonymous")
                ).WithPart("AutoroutePart", builder => builder
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "True")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "False")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{\"Name\":\"Title\",\"Pattern\":\"{Content.Slug}\",\"Description\":\"slugified-title\"}]"))
            );
            return 1;
         } catch (Exception e) {
            Logger.Error(e.Message);
            _notifier.Error(T(e.Message));
         }
         return 0;
      }

      public int UpdateFrom1() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord",
             table => table
                 .AddColumn("EditorMode", DbType.String));
         return 2;
      }

      public int UpdateFrom2() {
         SchemaBuilder.CreateTable(Common.PipelineSettingsName + "PartRecord", table => table
             .ContentPartRecord()
             .Column("EditorTheme", DbType.String)
         );

         return 3;
      }

      public int UpdateFrom3() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("Runnable", DbType.Boolean));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("Reportable", DbType.Boolean));
         return 4;
      }

      public int UpdateFrom4() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("NeedsInputFile", DbType.Boolean));
         return 5;
      }

      public int UpdateFrom5() {
         ContentDefinitionManager.AlterTypeDefinition(Common.PipelineConfigurationName, cfg => cfg.WithPart("TagsPart"));
         return 6;
      }

      public int UpdateFrom6() {
         ContentDefinitionManager.AlterTypeDefinition(Common.PipelineConfigurationName, cfg => cfg.WithSetting("Listable", "True"));
         return 7;
      }

      public int UpdateFrom7() {
         SchemaBuilder.AlterTable(Common.PipelineSettingsName + "PartRecord", table => table.AddColumn("MapBoxToken", DbType.String));
         return 8;
      }

      public int UpdateFrom8() {
         SchemaBuilder.AlterTable(Common.PipelineSettingsName + "PartRecord", table => table.AddColumn("MapBoxLimit", DbType.Int32));
         return 9;
      }

      public int UpdateFrom9() {
         try {
            var cfgs = _orchardServices.ContentManager.Query<PipelineConfigurationPart, PipelineConfigurationPartRecord>(VersionOptions.Published);
            foreach (var cfg in cfgs.List()) {
               if (!cfg.Migrated) {
                  cfg.Configuration = cfg.Record.Configuration;
                  cfg.Runnable = cfg.Record.Runnable;
                  cfg.NeedsInputFile = cfg.Record.NeedsInputFile;
                  cfg.StartAddress = cfg.Record.StartAddress;
                  cfg.EndAddress = cfg.Record.EndAddress;
                  cfg.EditorMode = cfg.Record.EditorMode;
                  cfg.Modes = string.Empty;
                  cfg.Migrated = true;
               }
            }

         } catch (Exception ex) {
            Logger.Warning(ex.Message);
         }

         return 10;
      }

      public int UpdateFrom10() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("Modes", DbType.String));
         return 11;
      }

      public int UpdateFrom11() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("PlaceHolderStyle", DbType.String));
         return 12;
      }

      public int UpdateFrom12() {
         SchemaBuilder.AlterTable(Common.PipelineSettingsName + "PartRecord", table => table.AddColumn("StartingLatitude", DbType.Double));
         SchemaBuilder.AlterTable(Common.PipelineSettingsName + "PartRecord", table => table.AddColumn("StartingLongitude", DbType.Double));
         return 13;
      }

      public int UpdateFrom13() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapStyle", DbType.String));
         return 14;
      }

      public int UpdateFrom14() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("ClientSideSorting", DbType.Boolean));
         return 15;
      }

      public int UpdateFrom15() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapCircleRadius", DbType.Int32));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapCircleOpacity", DbType.Double));
         return 16;
      }

      public int UpdateFrom16() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("PageSizes", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapSizes", DbType.String));
         return 17;
      }

      public int UpdateFrom17() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("EnableInlineParameters", DbType.Boolean));
         return 18;
      }

      public int UpdateFrom18() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("ClipTextAt", DbType.Int32));
         return 19;
      }

      public int UpdateFrom19() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("CalendarEnabled", DbType.Boolean));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("CalendarIdField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("CalendarTitleField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("CalendarUrlField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("CalendarClassField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("CalendarStartField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("CalendarEndField", DbType.String));

         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapEnabled", DbType.Boolean));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapColorField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapPopUpField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapLatitudeField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapLongitudeField", DbType.String));

         return 20;
      }

      public int UpdateFrom20() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("CalendarPaging", DbType.Boolean));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapPaging", DbType.Boolean));
         return 21;
      }

      public int UpdateFrom21() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapBulkActions", DbType.Boolean));
         return 22;
      }

      public int UpdateFrom22() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("MapZoom", DbType.Int32));
         return 23;
      }

      public int UpdateFrom23() {
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("ReportRowClassField", DbType.String));
         SchemaBuilder.AlterTable(Common.PipelineConfigurationName + "PartRecord", table => table.AddColumn("ReportRowStyleField", DbType.String));
         return 24;
      }
   }

   public class FileMigrations : DataMigrationImpl {
      public ILogger Logger { get; set; }
      public Localizer T { get; set; }
      private readonly INotifier _notifier;

      public FileMigrations(INotifier notifier) {
         Logger = NullLogger.Instance;
         T = NullLocalizer.Instance;
         _notifier = notifier;
      }

      public int Create() {

         try {
            SchemaBuilder.CreateTable(Common.PipelineFileName + "PartRecord", table => table
                .ContentPartRecord()
                .Column("FullPath", DbType.String)
                .Column("Direction", DbType.String)
            );
            ContentDefinitionManager.StoreTypeDefinition(
                new ContentTypeDefinition(Common.PipelineFileName, "Pipeline File")
            );
            ContentDefinitionManager.AlterTypeDefinition(Common.PipelineFileName, cfg => cfg
                .WithSetting("Description", "A file serving as input or output for a Transformalize pipeline.")
                .WithPart(Common.PipelineFileName + "Part")
                .WithPart("TitlePart")
                .WithPart("CommonPart")
                .WithPart("TagsPart")
                .WithPart("ContentPermissionsPart", builder => builder
                    .WithSetting("ContentPermissionsPartSettings.View", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.Publish", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.Edit", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.Delete", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.Preview", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.ViewOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.PublishOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.EditOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.DeleteOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.PreviewOwn", "Administrator")
                    .WithSetting("ContentPermissionsPartSettings.DisplayedRoles", "Authenticated,Anonymous")
            ));

            return 1;

         } catch (Exception ex) {
            Logger.Error(ex.Message);
            _notifier.Error(T(ex.Message));
         }
         return 0;
      }

   }

}