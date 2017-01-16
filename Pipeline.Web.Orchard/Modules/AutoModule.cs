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

using Autofac;
using Cfg.Net.Environment;
using Cfg.Net.Ext;
using Cfg.Net.Parsers;
using Cfg.Net.Parsers.YamlDotNet;
using Cfg.Net.Serializers;
using Cfg.Net.Shorthand;
using Orchard.FileSystems.AppData;
using Orchard.Logging;
using Orchard.Templates.Services;
using Orchard.UI.Notify;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Transform.Jint;
using Pipeline.Web.Orchard.Impl;
using Pipeline.Web.Orchard.Models;
using Transformalize;
using Transformalize.Impl;
using Transformalize.Transform.DateMath;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Pipeline.Web.Orchard.Modules {
    public class AutoModule : Module {

        public ILogger Logger { get; set; }

        public AutoModule() {
            Logger = NullLogger.Instance;
        }

        protected override void Load(ContainerBuilder builder) {


            builder.Register(c => {
                //var manager = c.Resolve<IShellSettingsManager>();
                //var settings = manager.LoadSettings().FirstOrDefault(s => !string.IsNullOrEmpty(s.DataProvider) && !string.IsNullOrEmpty(s.DataConnectionString));
                //if (settings == null) {
                //    Logger.Error("Transformalize (Pipeline.Web.Orchard) module could not read shell settings!  Default shorthand configuration used.");
                //    return Common.DefaultShortHand;
                //}

                //try {
                //    using (var cn = GetConnection(settings.DataProvider, settings.DataConnectionString)) {
                //        cn.Open();
                //        var cmd = cn.CreateCommand();
                //        cmd.CommandText = "SELECT ShortHand FROM Pipeline_Web_Orchard_PipelineSettingsPartRecord;";
                //        cmd.CommandType = CommandType.Text;
                //        var value = cmd.ExecuteScalar();
                //        if (value != null) {
                //            return value as string;
                //        }
                //    }
                //} catch (Exception ex) {
                //    Logger.Error("Tried to read short-hand configuration for Transformalize (Pipeline.Web.Orchard) module. {0}", ex.Message);
                //}
                return Common.DefaultShortHand;
            }).Named<string>("sh");

            builder.Register(c => new ShorthandRoot(c.ResolveNamed<string>("sh"))).As<ShorthandRoot>().SingleInstance();
            builder.Register(c => new ShorthandCustomizer(c.Resolve<ShorthandRoot>(), new [] {"fields","calculated-fields"},"t","transforms","method")).As<ShorthandCustomizer>();

            // xml
            builder.Register(c => new XmlProcess(
                new NanoXmlParser(),
                new XmlSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(),
                new IllegalCharacterValidator()
            )).As<XmlProcess>();

            builder.Register(c => new XmlToJsonProcess(
                new NanoXmlParser(),
                new JsonSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(), 
                new IllegalCharacterValidator()
            )).As<XmlToJsonProcess>();

            builder.Register(c => new XmlToYamlProcess(
                new NanoXmlParser(),
                new YamlDotNetSerializer(SerializationOptions.EmitDefaults, new CamelCaseNamingConvention()),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(),
                new IllegalCharacterValidator()
            )).As<XmlToYamlProcess>();

            builder.Register(c => new XmlProcessPass(new NanoXmlParser(), new XmlSerializer())).As<XmlProcessPass>();
            builder.Register(c => new XmlToJsonProcessPass(new NanoXmlParser(), new JsonSerializer())).As<XmlToJsonProcessPass>();
            builder.Register(c => new XmlToYamlProcessPass(new NanoXmlParser(), new YamlDotNetSerializer(SerializationOptions.EmitDefaults, new CamelCaseNamingConvention()))).As<XmlToYamlProcessPass>();

            // json
            builder.Register(c => new JsonProcess(
                new FastJsonParser(),
                new JsonSerializer(),
                new JintValidator(),
                //new OrchardNodeModifier("host", c.Resolve<IOrchardServices>()),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(),
                new IllegalCharacterValidator()
            )).As<JsonProcess>();

            builder.Register(c => new JsonToXmlProcess(
                new FastJsonParser(),
                new XmlSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(),
                new IllegalCharacterValidator()
            )).As<JsonToXmlProcess>();

            builder.Register(c => new JsonToYamlProcess(
                new FastJsonParser(),
                new YamlDotNetSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(),
                new IllegalCharacterValidator()
            )).As<JsonToYamlProcess>();

            builder.Register(c => new JsonProcessPass(new FastJsonParser(), new JsonSerializer())).As<JsonProcessPass>();
            builder.Register(c => new JsonToXmlProcessPass(new FastJsonParser(), new XmlSerializer())).As<JsonToXmlProcessPass>();
            builder.Register(c => new JsonToYamlProcessPass(new FastJsonParser(), new YamlDotNetSerializer())).As<JsonToYamlProcessPass>();

            // yaml
            builder.Register(c => new YamlProcess(
                new YamlDotNetParser(),
                new YamlDotNetSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(),
                new IllegalCharacterValidator()
            )).As<YamlProcess>();

            builder.Register(c => new YamlToXmlProcess(
                new YamlDotNetParser(),
                new XmlSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(),
                new IllegalCharacterValidator()
            )).As<YamlToXmlProcess>();

            builder.Register(c => new YamlToJsonProcess(
                new YamlDotNetParser(),
                new JsonSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new DateMathModifier(),
                new EnvironmentModifier(),
                new IllegalCharacterValidator()
            )).As<YamlToJsonProcess>();

            builder.Register(c => new YamlProcessPass(new YamlDotNetParser(), new YamlDotNetSerializer())).As<YamlProcessPass>();
            builder.Register(c => new YamlToXmlProcessPass(new YamlDotNetParser(), new XmlSerializer())).As<YamlToXmlProcessPass>();
            builder.Register(c => new YamlToJsonProcessPass(new YamlDotNetParser(), new JsonSerializer())).As<YamlToJsonProcessPass>();

            var logger = new OrchardLogger();
            var context = new PipelineContext(logger, new Process { Name = "OrchardCMS" }.WithDefaults());

            builder.Register(c => new RunTimeDataReader(logger, c.Resolve<IAppDataFolder>(), c.Resolve<ITemplateProcessor>(), c.Resolve<INotifier>())).As<IRunTimeRun>();
            builder.Register(c => new CachingRunTimeSchemaReader(new RunTimeSchemaReader(context, c.Resolve<IAppDataFolder>(), c.Resolve<ITemplateProcessor>(), c.Resolve<INotifier>()))).As<IRunTimeSchemaReader>();
            builder.Register(c => new SchemaHelper(context, c.Resolve<IRunTimeSchemaReader>())).As<ISchemaHelper>();
            builder.Register(c => new RunTimeExecuter(context, c.Resolve<IAppDataFolder>(), c.Resolve<ITemplateProcessor>(), c.Resolve<INotifier>())).As<IRunTimeExecute>();

        }

        //private IDbConnection GetConnection(string provider, string connectionString) {
        //    var providerTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
        //        {"SqlServer", "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, 
        //        {"SqlCe", "System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe"}, 
        //        {"MySql", "MySql.Data.MySqlClient.MySqlConnection, MySql.Data"},
        //        {"PostgreSql","Npgsql.NpgsqlConnection, Npgsql"}
        //    };

        //    if (!providerTypes.ContainsKey(provider)) {
        //        Logger.Warning("Transformalize for Orchard CMS may not use the {0} provider to retrieve shorthand configuration.  It can only handle SqlServer, SqlCe, PostgreSql, or MySql. The default short-hand configuration is used when this happens.");
        //        return null;
        //    }

        //    var type = Type.GetType(providerTypes[provider.ToLower()], false, true);
        //    var connection = (IDbConnection)Activator.CreateInstance(type);
        //    connection.ConnectionString = connectionString;
        //    return connection;
        //}

    }
}
