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

                SchemaBuilder.CreateTable("PipelineConfigurationPartRecord", table => table
                    .ContentPartRecord()
                    .Column("Configuration", DbType.String, column => column.Unlimited())
                    .Column("StartAddress", DbType.String)
                    .Column("EndAddress", DbType.String)
                );

                ContentDefinitionManager.StoreTypeDefinition(
                    new ContentTypeDefinition("PipelineConfiguration", "Pipeline.Net")
                );

                ContentDefinitionManager.AlterTypeDefinition("PipelineConfiguration", cfg => cfg
                    .Creatable()
                    .WithPart("PipelineConfigurationPart")
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
                    .WithPart("IdentityPart")
                    .WithPart("ContentPermissionsPart", builder => builder
                        .WithSetting("ContentPermissionsPartSettings.View", "Administrator")
                        .WithSetting("ContentPermissionsPartSettings.Publish", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.Edit", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.Delete", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.Preview", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.ViewOwn", "Administrator")
                        .WithSetting("ContentPermissionsPartSettings.PublishOwn", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.EditOwn", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.DeleteOwn", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.PreviewOwn", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.DisplayedRoles", "Authenticated,Anonymous")
                    )
                );

            } catch (Exception e) {
                Logger.Error("Error Creating Pipeline.Net Parts. {0}", e.Message);
            }
            return 1;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("PipelineConfigurationPartRecord",
                table => table
                    .AddColumn("EditorMode", DbType.String));
            return 2;
        }

    }
}