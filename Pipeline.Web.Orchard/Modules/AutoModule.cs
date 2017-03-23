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
using Cfg.Net.Parsers;
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
using Transformalize.Transform.DateMath;

namespace Pipeline.Web.Orchard.Modules {
    public class AutoModule : Module {

        public ILogger Logger { get; set; }

        public AutoModule() {
            Logger = NullLogger.Instance;
        }

        protected override void Load(ContainerBuilder builder) {

            builder.Register(c => new ShorthandRoot(Common.DefaultShortHand)).As<ShorthandRoot>().SingleInstance();
            builder.Register(c => new ShorthandCustomizer(c.Resolve<ShorthandRoot>(), new [] {"fields","calculated-fields"},"t","transforms","method")).As<ShorthandCustomizer>();

            // xml
            builder.Register(c => new XmlProcess(
                new DateMathModifier(),
                new NanoXmlParser(),
                new XmlSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new EnvironmentModifier()
            )).As<XmlProcess>();

            builder.Register(c => new XmlToJsonProcess(
                new DateMathModifier(),
                new NanoXmlParser(),
                new JsonSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new EnvironmentModifier()
            )).As<XmlToJsonProcess>();

            builder.Register(c => new XmlProcessPass(new NanoXmlParser(), new XmlSerializer())).As<XmlProcessPass>();
            builder.Register(c => new XmlToJsonProcessPass(new NanoXmlParser(), new JsonSerializer())).As<XmlToJsonProcessPass>();

            // json
            builder.Register(c => new JsonProcess(
                new DateMathModifier(),
                new FastJsonParser(),
                new JsonSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new EnvironmentModifier()
            )).As<JsonProcess>();

            builder.Register(c => new JsonToXmlProcess(
                new DateMathModifier(),
                new FastJsonParser(),
                new XmlSerializer(),
                new JintValidator(),
                c.Resolve<ShorthandCustomizer>(),
                new EnvironmentModifier()
            )).As<JsonToXmlProcess>();

            builder.Register(c => new JsonProcessPass(new FastJsonParser(), new JsonSerializer())).As<JsonProcessPass>();
            builder.Register(c => new JsonToXmlProcessPass(new FastJsonParser(), new XmlSerializer())).As<JsonToXmlProcessPass>();

            var logger = new OrchardLogger();
            var context = new PipelineContext(logger, new Process { Name = "OrchardCMS" });

            builder.Register(c => new RunTimeDataReader(logger, c.Resolve<IAppDataFolder>(), c.Resolve<ITemplateProcessor>(), c.Resolve<INotifier>())).As<IRunTimeRun>();
            builder.Register(c => new CachingRunTimeSchemaReader(new RunTimeSchemaReader(context, c.Resolve<IAppDataFolder>(), c.Resolve<ITemplateProcessor>(), c.Resolve<INotifier>()))).As<IRunTimeSchemaReader>();
            builder.Register(c => new SchemaHelper(context, c.Resolve<IRunTimeSchemaReader>())).As<ISchemaHelper>();
            builder.Register(c => new RunTimeExecuter(context, c.Resolve<IAppDataFolder>(), c.Resolve<ITemplateProcessor>(), c.Resolve<INotifier>())).As<IRunTimeExecute>();

        }

    }
}
