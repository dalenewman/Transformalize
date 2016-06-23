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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autofac;
using Cfg.Net.Environment;
using Cfg.Net.Ext;
using Cfg.Net.Parsers;
using Cfg.Net.Parsers.YamlDotNet;
using Cfg.Net.Serializers;
using Cfg.Net.Shorthand;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Scripting.Jint;
using Pipeline.Web.Orchard.Impl;
using Pipeline.Web.Orchard.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Pipeline.Web.Orchard.Modules {
    public class AutoModule : Module {

        public static string _defaultShortHand = @"<cfg>
  <signatures>
    <add name='none' />
    <add name='format'>
      <parameters>
        <add name='format' />
      </parameters>
    </add>
    <add name='length'>
      <parameters>
        <add name='length' />
      </parameters>
    </add>
    <add name='separator'>
      <parameters>
        <add name='separator' />
      </parameters>
    </add>
    <add name='padding'>
      <parameters>
        <add name='total-width' />
        <add name='padding-char' value='0' />
      </parameters>
    </add>
    <add name='timezone'>
      <parameters>
        <add name='from-time-zone' />
        <add name='to-time-zone' />
      </parameters>
    </add>
    <add name='fromtimezone'>
      <parameters>
        <add name='from-time-zone' value='UTC' />
      </parameters>
    </add>
    <add name='value'>
      <parameters>
        <add name='value' />
      </parameters>
    </add>
    <add name='type'>
      <parameters>
        <add name='type' value='[default]' />
      </parameters>
    </add>
    <add name='trim'>
      <parameters>
        <add name='trim-chars' value=' ' />
      </parameters>
    </add>
    <add name='script'>
      <parameters>
        <add name='script'  />
      </parameters>
    </add>
    <add name='map'>
      <parameters>
        <add name='map' />
      </parameters>
    </add>
    <add name='dayofweek'>
      <parameters>
        <add name='dayofweek' />
      </parameters>
    </add>
    <add name='substring'>
      <parameters>
        <add name='startindex' />
        <add name='length' value='0' />
      </parameters>
    </add>
    <add name='timecomponent'>
      <parameters>
        <add name='timecomponent' />
      </parameters>
    </add>
    <add name='replace'>
      <parameters>
        <add name='oldvalue' />
        <add name='newvalue' value='' />
      </parameters>
    </add>
    <add name='regexreplace'>
      <parameters>
        <add name='pattern' />
        <add name='newvalue' />
        <add name='count' value='0' />
      </parameters>
    </add>
    <add name='insert'>
      <parameters>
        <add name='startindex' />
        <add name='value' />
      </parameters>
    </add>
    <add name='remove'>
      <parameters>
        <add name='startindex' />
        <add name='count' value='0' />
      </parameters>
    </add>
    <add name='razor'>
      <parameters>
        <add name='template' />
        <add name='contenttype' value='raw' />
      </parameters>
    </add>
    <add name='any'>
      <parameters>
        <add name='value'/>
        <add name='operator' value='equal' />
      </parameters>
    </add>
    <add name='property'>
      <parameters>
        <add name='name' />
        <add name='property' />
      </parameters>
    </add>
    <add name='file'>
      <parameters>
        <add name='extension' value='true'/>
      </parameters>
    </add>
    <add name='xpath'>
      <parameters>
        <add name='xpath' />
        <add name='namespace' value='' />
        <add name='url' value='' />
      </parameters>
    </add>
    <add name='datediff'>
      <parameters>
        <add name='timecomponent' />
        <add name='fromtimezone' value='UTC' />
      </parameters>
    </add>
  </signatures>

  <targets>
    <add name='t' collection='transforms' property='method' />
    <add name='ignore' collection='' property='' />
  </targets>

  <methods>
    <add name='add' signature='none' target='t' />
    <add name='any' signature='any' target='t' />
    <add name='concat' signature='none' target='t' />
    <add name='connection' signature='property' target='t' />
    <add name='contains' signature='value' target='t' />
    <add name='convert' signature='type' target='t' />
    <add name='copy' signature='none' target='ignore' />
    <add name='cs' signature='script' target='t' />
    <add name='csharp' signature='script' target='t' />
    <add name='datediff' signature='datediff' target='t' />
    <add name='datepart' signature='timecomponent' target='t' />
    <add name='decompress' signature='none' target='t' />
    <add name='fileext' signature='none' target='t' />
    <add name='filename' signature='file' target='t' />
    <add name='filepath' signature='file' target='t' />
    <add name='format' signature='format' target='t' />
    <add name='formatphone' signature='none' target='t' />
    <add name='hashcode' signature='none' target='t' />
    <add name='htmldecode' signature='none' target='t' />
    <add name='insert' signature='insert' target='t' />
    <add name='is' signature='type' target='t' />
    <add name='javascript' signature='script' target='t' />
    <add name='join' signature='separator' target='t' />
    <add name='js' signature='script' target='t' />
    <add name='last' signature='dayofweek' target='t' />
    <add name='left' signature='length' target='t' />
    <add name='lower' signature='none' target='t' />
    <add name='map' signature='map' target='t' />
    <add name='multiply' signature='none' target='t' />
    <add name='next' signature='dayofweek' target='t' />
    <add name='now' signature='none' target='t' />
    <add name='padleft' signature='padding' target='t' />
    <add name='padright' signature='padding' target='t' />
    <add name='razor' signature='razor' target='t' />
    <add name='regexreplace' signature='regexreplace' target='t' />
    <add name='remove' signature='remove' target='t' />
    <add name='replace' signature='replace' target='t' />
    <add name='right' signature='length' target='t' />
    <add name='splitlength' signature='separator' target='t' />
    <add name='substring' signature='substring' target='t' />
    <add name='sum' signature='none' target='t' />
    <add name='timeago' signature='fromtimezone' target='t' />
    <add name='timeahead' signature='fromtimezone' target='t' />
    <add name='timezone' signature='timezone' target='t' />
    <add name='tolower' signature='none' target='t' />
    <add name='tostring' signature='format' target='t' />
    <add name='totime' signature='timecomponent' target='t' />
    <add name='toupper' signature='none' target='t' />
    <add name='toyesno' signature='none' target='t' />
    <add name='trim' signature='trim' target='t' />
    <add name='trimend' signature='trim' target='t' />
    <add name='trimstart' signature='trim' target='t' />
    <add name='upper' signature='none' target='t' />
    <add name='utcnow' signature='none' target='t' />
    <add name='xmldecode' signature='none' target='t' />
    <add name='xpath' signature='xpath' target='t' />
  </methods>

</cfg>";
        public ILogger Logger { get; set; }

        public AutoModule() {
            Logger = NullLogger.Instance;
        }

        protected override void Load(ContainerBuilder builder) {

            builder.Register(c => {
                var manager = c.Resolve<IShellSettingsManager>();
                var settings = manager.LoadSettings().FirstOrDefault(s => !string.IsNullOrEmpty(s.DataProvider) && !string.IsNullOrEmpty(s.DataConnectionString));
                if (settings == null) {
                    Logger.Error("Transformalize (Pipeline.Web.Orchard) module could not read shell settings!  Default shorthand configuration used.");
                    return _defaultShortHand;
                }

                try {
                    using (var cn = GetConnection(settings.DataProvider, settings.DataConnectionString)) {
                        cn.Open();
                        var cmd = cn.CreateCommand();
                        cmd.CommandText = "SELECT ShortHand FROM Pipeline_Web_Orchard_PipelineSettingsPartRecord;";
                        cmd.CommandType = CommandType.Text;
                        var value = cmd.ExecuteScalar();
                        if (value != null) {
                            return value as string;
                        }
                    }
                } catch (Exception ex) {
                    Logger.Error("Tried to read short-hand configuration for Transformalize (Pipeline.Web.Orchard) module. {0}", ex.Message);
                }
                return _defaultShortHand;
            }).Named<string>("sh");

            builder.Register(c => new ShorthandRoot(c.ResolveNamed<string>("sh"))).As<ShorthandRoot>().SingleInstance();
            builder.Register(c => new ShorthandValidator(c.Resolve<ShorthandRoot>(), "sh")).As<ShorthandValidator>();
            builder.Register(c => new ShorthandModifier(c.Resolve<ShorthandRoot>(), "sh")).As<ShorthandModifier>();

            builder.Register(c => new XmlProcess(
                new NanoXmlParser(),
                new XmlSerializer(),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
                )).As<XmlProcess>();

            builder.Register(c => new XmlToJsonProcess(
                new NanoXmlParser(),
                new JsonSerializer(),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
                )).As<XmlToJsonProcess>();

            builder.Register(c => new XmlToYamlProcess(
                new NanoXmlParser(),
                new YamlDotNetSerializer(SerializationOptions.EmitDefaults, new CamelCaseNamingConvention()),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
                )).As<XmlToYamlProcess>();


            builder.Register(c => new JsonProcess(
                new FastJsonParser(),
                new JsonSerializer(),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
            )).As<JsonProcess>();

            builder.Register(c => new JsonToXmlProcess(
                new FastJsonParser(),
                new XmlSerializer(),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
            )).As<JsonToXmlProcess>();

            builder.Register(c => new JsonToYamlProcess(
                new FastJsonParser(),
                new YamlDotNetSerializer(),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
            )).As<JsonToYamlProcess>();

            builder.Register(c => new YamlProcess(
                new YamlDotNetParser(),
                new YamlDotNetSerializer(),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
            )).As<YamlProcess>();

            builder.Register(c => new YamlToXmlProcess(
                new YamlDotNetParser(),
                new XmlSerializer(),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
            )).As<YamlToXmlProcess>();

            builder.Register(c => new YamlToJsonProcess(
                new YamlDotNetParser(),
                new JsonSerializer(),
                new JintValidator("js"),
                c.Resolve<ShorthandValidator>(),
                c.Resolve<ShorthandModifier>(),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderValidator()
            )).As<YamlToJsonProcess>();

            var logger = new OrchardLogger();
            var context = new PipelineContext(logger, new Process { Name = "OrchardCMS" }.WithDefaults());

            builder.Register(c => new RunTimeDataReader(logger)).As<IRunTimeRun>();
            builder.Register(c => new CachingRunTimeSchemaReader(new RunTimeSchemaReader(context))).As<IRunTimeSchemaReader>();
            builder.Register(c => new SchemaHelper(context, c.Resolve<IRunTimeSchemaReader>())).As<ISchemaHelper>();
            builder.Register(c => new RunTimeExecuter(context)).As<IRunTimeExecute>();

        }

        private IDbConnection GetConnection(string provider, string connectionString) {
            var providerTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {"SqlServer", "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}, 
                {"SqlCe", "System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe"}, 
                {"MySql", "MySql.Data.MySqlClient.MySqlConnection, MySql.Data"},
                {"PostgreSql","Npgsql.NpgsqlConnection, Npgsql"}
            };

            if (!providerTypes.ContainsKey(provider)) {
                Logger.Warning("Transformalize for Orchard CMS may not use the {0} provider to retrieve shorthand configuration.  It can only handle SqlServer, SqlCe, PostgreSql, or MySql. The default short-hand configuration is used when this happens.");
                return null;
            }

            var type = Type.GetType(providerTypes[provider.ToLower()], false, true);
            var connection = (IDbConnection)Activator.CreateInstance(type);
            connection.ConnectionString = connectionString;
            return connection;
        }

    }
}
