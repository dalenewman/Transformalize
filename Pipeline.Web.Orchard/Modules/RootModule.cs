#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Cfg.Net.Contracts;
using Cfg.Net.Environment;
using Cfg.Net.Reader;
using Orchard.Localization;
using Orchard.Logging;
using System.Collections.Generic;
using System.Linq;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Transforms.DateMath;
using IDependency = Cfg.Net.Contracts.IDependency;

// ReSharper disable PossibleMultipleEnumeration

namespace Pipeline.Web.Orchard.Modules {

    public class RootModule : Module {
        public Localizer T { get; set; }
        public global::Orchard.Logging.ILogger Logger { get; set; }

        public RootModule() {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        protected override void Load(ContainerBuilder builder) {

            builder.RegisterType<Cfg.Net.Serializers.XmlSerializer>().As<ISerializer>();

            builder.Register(ctx => {

                // This reader is used to load the initial configuration and nested resources for tfl actions, etc.
                builder.Register(c => new DefaultReader(new FileReader(), new WebReader())).As<IReader>();

                var dependencies = new List<IDependency> {
                    ctx.Resolve<IReader>(),
                    ctx.Resolve<ISerializer>(),
                    new DateMathModifier(),
                    new EnvironmentModifier(),
                    new IllegalCharacterValidator()
                };

                if (ctx.IsRegistered<FieldTransformShorthandCustomizer>()) {
                    dependencies.Add(ctx.Resolve<FieldTransformShorthandCustomizer>());
                }

                if (ctx.IsRegistered<ValidateShorthandCustomizer>()) {
                    dependencies.Add(ctx.Resolve<ValidateShorthandCustomizer>());
                }

                var process = new Process(dependencies.ToArray());

                if (!ctx.IsRegisteredWithName<string>("cfg")) {
                    process.Name = "Error";
                    process.Status = 500;
                    process.Message = "The configuration (cfg) is not registered in the container.";
                    process.Check();
                    return process;
                }

                process.Load(ctx.ResolveNamed<string>("cfg"));

                // this might be put into it's own type and injected (or not)
                if (process.Entities.Count == 1) {
                    var entity = process.Entities.First();
                    if (!entity.HasInput() && ctx.IsRegistered<ISchemaReader>()) {
                        var schemaReader = ctx.Resolve<ISchemaReader>(new TypedParameter(typeof(Process), process));
                        var schema = schemaReader.Read(entity);
                        var newEntity = schema.Entities.First();
                        foreach (var sf in newEntity.Fields.Where(f => f.Name == Constants.TflKey || f.Name == Constants.TflDeleted || f.Name == Constants.TflBatchId || f.Name == Constants.TflHashCode)) {
                            sf.Alias = newEntity.Name + sf.Name;
                        }
                        process.Entities.Clear();
                        process.Entities.Add(newEntity);
                        process.Connections.First(c => c.Name == newEntity.Connection).Delimiter = schema.Connection.Delimiter;
                        process = new Process(process.Serialize(), ctx.Resolve<ISerializer>());
                    }
                }

                return process;
            }).As<Process>().InstancePerDependency();  // because it has state, if you run it again, it's not so good

        }
    }
}
