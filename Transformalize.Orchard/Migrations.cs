using System;
using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Logging;

namespace Transformalize.Orchard {

    public class Migrations : DataMigrationImpl {
        public ILogger Logger { get; set; }

        public Migrations() {
            Logger = NullLogger.Instance;
        }

        public int Create() {

            try {

                SchemaBuilder.CreateTable("ConfigurationPartRecord", table => table
                    .ContentPartRecord()
                    .Column("Configuration", DbType.String, column => column.Unlimited())
                    .Column("TryCatch", DbType.Boolean)
                    .Column("DisplayLog", DbType.Boolean)
                );
                SchemaBuilder.CreateTable("FilePartRecord", table => table
                    .ContentPartRecord()
                    .Column("FullPath", DbType.String)
                    .Column("Direction", DbType.String)
                );

                ContentDefinitionManager.StoreTypeDefinition(
                    new ContentTypeDefinition("Configuration", "Transformalize")
                );
                ContentDefinitionManager.StoreTypeDefinition(
                    new ContentTypeDefinition("File", "File")
                );

                ContentDefinitionManager.AlterTypeDefinition("Configuration", cfg => cfg
                    .Creatable()
                    .WithPart("ConfigurationPart")
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
                    .WithPart("IdentityPart")
                    .WithPart("ContentPermissionsPart", builder => builder
                        .WithSetting("ContentPermissionsPartSettings.View", "Administrator")
                        .WithSetting("ContentPermissionsPartSettings.Publish", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.Edit", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.Delete", "Adminstrator")
                        .WithSetting("ContentPermissionsPartSettings.DisplayedRoles", "Authenticated,Anonymous")
                    )
                );
                ContentDefinitionManager.AlterTypeDefinition("File", cfg => cfg
                    .WithPart("FilePart")
                    .WithPart("CommonPart")
                );

            } catch (Exception e) {
                Logger.Error("Creating Transformalize Configuration. Error Message: {0}", e.Message);
            }
            return 1;
        }

        public int UpdateFrom1() {
            try {

                SchemaBuilder.AlterTable("ConfigurationPartRecord",
                    table => table
                        .AddColumn("Modes", DbType.String));
            } catch (Exception e) {
                Logger.Error("Creating Transformalize Configuration. Error Message: {0}", e.Message);
            }

            return 2;
        }

        public int UpdateFrom2() {
            try {
                SchemaBuilder.AlterTable("ConfigurationPartRecord",
                    table => table
                        .AddColumn("LogLevel", DbType.String));
            } catch (Exception e) {
                Logger.Error("Creating Transformalize Configuration. Error Message: {0}", e.Message);
            }

            return 3;
        }

        public int UpdateFrom3() {
            try {
                SchemaBuilder.AlterTable("ConfigurationPartRecord",
                    table => table
                        .AddColumn("OutputFileExtension", DbType.String));

            } catch (Exception e) {
                Logger.Error("Creating Transformalize Configuration. Error Message: {0}", e.Message);
            }

            return 4;
        }

        public int UpdateFrom4() {
            try {
                SchemaBuilder.AlterTable("ConfigurationPartRecord",
                    table => table
                        .AddColumn("StartAddress", DbType.String));
                SchemaBuilder.AlterTable("ConfigurationPartRecord",
                    table => table
                        .AddColumn("EndAddress", DbType.String));
            } catch (Exception e) {
                Logger.Error("Creating Transformalize Configuration. Error Message: {0}", e.Message);
            }
            return 5;
        }

        public int UpdateFrom5() {
            try {
                SchemaBuilder.AlterTable("ConfigurationPartRecord",
                    table => table
                        .AddColumn("EditorTheme", DbType.String));
            } catch (Exception e) {
                Logger.Error("Creating Transformalize Configuration. Error Message: {0}", e.Message);
            }
            return 6;
        }

    }
}