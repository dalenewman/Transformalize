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
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Logging;

namespace Pipeline.Web.Orchard {

    public class Migrations : DataMigrationImpl {
        public ILogger Logger { get; set; }

        public Migrations() {
            Logger = NullLogger.Instance;
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
                    .WithSetting("Description", "An arrangement for a Transformalize pipeline composition.")
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
                    )
                );

            } catch (Exception e) {
                Logger.Error("Error Creating Pipeline.Net Parts. {0}", e.Message);
            }
            return 1;
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
                .Column("Shorthand", DbType.String, column => column.Unlimited())
            );

            return 3;
        }

        public int UpdateFrom3() {
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
            return 4;
        }

    }
}