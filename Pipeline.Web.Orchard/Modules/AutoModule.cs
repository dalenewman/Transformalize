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

using System.IO;
using System.Reflection;
using Autofac;
using Cfg.Net.Environment;
using Cfg.Net.Ext;
using Cfg.Net.Parsers;
using Cfg.Net.Parsers.YamlDotNet;
using Cfg.Net.Serializers;
using Cfg.Net.Shorthand;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Scripting.Jint;
using Pipeline.Web.Orchard.Impl;
using Pipeline.Web.Orchard.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Module = Autofac.Module;

namespace Pipeline.Web.Orchard.Modules {
    public class AutoModule : Module {

        protected override void Load(ContainerBuilder builder) {

            var assembly = Assembly.GetExecutingAssembly();
            string cfg;
            using (var stream = assembly.GetManifestResourceStream("Pipeline.Web.Orchard.Shorthand.xml"))
            using (var reader = new StreamReader(stream)) {
                cfg = reader.ReadToEnd();
            }

            builder.Register((c) => cfg).Named<string>("sh");

            var sh = new ShorthandRoot(cfg);

            builder.Register(c => new XmlProcess(
                new NanoXmlParser(),
                new XmlSerializer(),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
                )).As<XmlProcess>();

            builder.Register(c => new XmlToJsonProcess(
                new NanoXmlParser(),
                new JsonSerializer(),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
                )).As<XmlToJsonProcess>();

            builder.Register(c => new XmlToYamlProcess(
                new NanoXmlParser(),
                new YamlDotNetSerializer(SerializationOptions.EmitDefaults, new CamelCaseNamingConvention()),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
                )).As<XmlToYamlProcess>();


            builder.Register(c => new JsonProcess(
                new FastJsonParser(),
                new JsonSerializer(),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
            )).As<JsonProcess>();

            builder.Register(c => new JsonToXmlProcess(
                new FastJsonParser(),
                new XmlSerializer(),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
            )).As<JsonToXmlProcess>();

            builder.Register(c => new JsonToYamlProcess(
                new FastJsonParser(),
                new YamlDotNetSerializer(),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
            )).As<JsonToYamlProcess>();

            builder.Register(c => new YamlProcess(
                new YamlDotNetParser(),
                new YamlDotNetSerializer(),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
            )).As<YamlProcess>();

            builder.Register(c => new YamlToXmlProcess(
                new YamlDotNetParser(),
                new XmlSerializer(),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
            )).As<YamlToXmlProcess>();

            builder.Register(c => new YamlToJsonProcess(
                new YamlDotNetParser(),
                new JsonSerializer(),
                new JintValidator("js"),
                new ShorthandValidator(sh, "sh"),
                new ShorthandModifier(sh, "sh"),
                new PlaceHolderModifier(),
                new EnvironmentModifier(new PlaceHolderModifier(), new Cfg.Net.Environment.ParameterModifier()),
                new PlaceHolderValidator()
            )).As<YamlToJsonProcess>();

            var logger = new OrchardLogger();
            var context = new PipelineContext(logger, new Process { Name = "OrchardCMS" }.WithDefaults());

            builder.Register(c => new RunTimeDataReader(logger)).As<IRunTimeRun>();
            builder.Register(c => new CachingRunTimeSchemaReader(new RunTimeSchemaReader(context))).As<IRunTimeSchemaReader>();
            builder.Register(c => new SchemaHelper(context, c.Resolve<IRunTimeSchemaReader>())).As<ISchemaHelper>();
            builder.Register(c => new RunTimeExecuter(context)).As<IRunTimeExecute>();

        }
    }
}
