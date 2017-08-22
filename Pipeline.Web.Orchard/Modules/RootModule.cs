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

using System.Collections.Generic;
using System.Linq;
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Environment;
using Cfg.Net.Reader;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms.Jint;
using Orchard.Localization;
using Transformalize;
using Transformalize.Impl;
using Transformalize.Transforms.DateMath;

// ReSharper disable PossibleMultipleEnumeration

namespace Pipeline.Web.Orchard.Modules {

    public class RootModule : Module {
        public Localizer T { get; set; }

        public RootModule() {
            T = NullLocalizer.Instance;
        }

        protected override void Load(ContainerBuilder builder) {

            builder.RegisterType<Cfg.Net.Serializers.XmlSerializer>().As<ISerializer>();
            builder.Register(ctx => new JintValidator()).Named<ICustomizer>("js");

            // This reader is used to load the initial configuration and nested resources for tfl actions, etc.
            builder.RegisterType<FileReader>().Named<IReader>("file");
            builder.RegisterType<WebReader>().Named<IReader>("web");
            builder.Register<IReader>(ctx => new DefaultReader(
                ctx.ResolveNamed<IReader>("file"),
                new ReTryingReader(ctx.ResolveNamed<IReader>("web"), attempts: 3))
            );

            builder.Register((ctx, p) => {

                var dependencies = new List<IDependency> {
                    ctx.Resolve<IReader>(),
                    ctx.Resolve<ISerializer>(),
                    new DateMathModifier(),
                    new EnvironmentModifier(),
                    ctx.ResolveNamed<ICustomizer>("js"),
                    new IllegalCharacterValidator()
                };

                if (ctx.IsRegisteredWithName<IDependency>("shorthand")) {
                    dependencies.Add(ctx.ResolveNamed<IDependency>("shorthand"));
                }

                var process = new Process(dependencies.ToArray());

                switch (p.Count()) {
                    case 2:
                        process.Load(
                            p.Named<string>("cfg"),
                            p.Named<Dictionary<string, string>>("parameters")
                        );
                        break;
                    default:
                        process.Load(p.Named<string>("cfg"));
                        break;
                }

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
