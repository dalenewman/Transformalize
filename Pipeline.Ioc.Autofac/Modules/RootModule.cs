#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
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
using Cfg.Net.Ext;
using Cfg.Net.Reader;
using Cfg.Net.Shorthand;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Desktop.Loggers;
using Pipeline.Nulls;
using Pipeline.Scripting.Jint;
using Pipeline.Template.Razor;
using IParser = Pipeline.Contracts.IParser;

// ReSharper disable PossibleMultipleEnumeration

namespace Pipeline.Ioc.Autofac.Modules {

    public class RootModule : Module {
        private readonly string _shorthand;

        public RootModule() { }

        public RootModule(string shorthand) {
            _shorthand = shorthand;
        }

        protected override void Load(ContainerBuilder builder) {

            builder.Register(ctx => new JintValidator("js")).Named<IValidator>("js");

            builder.Register(ctx => new EnvironmentModifier(
                new PlaceHolderModifier(),
                new ParameterModifier())
            ).As<IRootModifier>();

            // This reader is used to load the initial configuration and nested resources for tfl actions, etc.
            builder.RegisterType<FileReader>().Named<IReader>("file");
            builder.RegisterType<WebReader>().Named<IReader>("web");
            builder.Register<IReader>(ctx => new DefaultReader(
                ctx.ResolveNamed<IReader>("file"),
                new ReTryingReader(ctx.ResolveNamed<IReader>("web"), attempts: 3))
            );

            // transform choices
            builder.Register<ITransform>((ctx, p) => new JintTransform(p.TypedAs<PipelineContext>(), ctx.Resolve<IReader>())).Named<ITransform>("js");
            builder.Register<ITransform>((ctx, p) => new RazorTransform(p.TypedAs<PipelineContext>())).Named<ITransform>("razor");

            // parser choices
            builder.RegisterType<JintParser>().Named<IParser>("js");

            // input row condition
            builder.Register<IRowCondition>((ctx, p) => new JintRowCondition(p.TypedAs<InputContext>(), p.TypedAs<string>())).As<IRowCondition>();

            builder.Register((ctx, p) => {

                var dependencies = new List<IDependency> {
                    ctx.Resolve<IReader>(),
                    new PlaceHolderModifier(),
                    ctx.Resolve<IRootModifier>(),
                    ctx.ResolveNamed<IValidator>("js"),
                    new PlaceHolderValidator()
                };

                if (!string.IsNullOrEmpty(_shorthand)) {
                    var shr = new ShorthandRoot(_shorthand, ctx.ResolveNamed<IReader>("file"));
                    if (shr.Errors().Any()) {
                        var context = ctx.IsRegistered<IContext>() ? ctx.Resolve<IContext>() : new PipelineContext(ctx.IsRegistered<IPipelineLogger>() ? ctx.Resolve<IPipelineLogger>() : new TraceLogger(), new Process { Name = "Error" }.WithDefaults());
                        foreach (var error in shr.Errors()) {
                            context.Error(error);
                        }
                        context.Error("Please fix you shorthand configuration.  No short-hand is being processed.");
                        dependencies.Add(new NullValidator("sh"));
                        dependencies.Add(new NullNodeModifier("sh"));
                    } else {
                        dependencies.Add(new ShorthandValidator(shr, "sh"));
                        dependencies.Add(new ShorthandModifier(shr, "sh"));
                    }
                } else {
                    dependencies.Add(new NullValidator("sh"));
                    dependencies.Add(new NullNodeModifier("sh"));
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
                        var newEntity = schemaReader.Read(entity).Entities.First();
                        foreach (var sf in newEntity.Fields.Where(f => f.Name == Constants.TflKey || f.Name == Constants.TflDeleted || f.Name == Constants.TflBatchId || f.Name == Constants.TflHashCode)) {
                            sf.Alias = newEntity.Name + sf.Name;
                        }
                        process.Entities.Clear();
                        process.Entities.Add(newEntity);
                        process = new Process(process.Serialize(), ctx.Resolve<ISerializer>());
                    }
                }

                return process;
            }).As<Process>().InstancePerDependency();  // because it has state, if you run it again, it's not so good

        }
    }
}
