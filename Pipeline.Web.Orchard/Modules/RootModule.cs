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
using Cfg.Net.Ext;
using Cfg.Net.Reader;
using Cfg.Net.Shorthand;
using Orchard.Templates.Services;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Transform.Jint;
using Pipeline.Web.Orchard.Impl;
using IParser = Transformalize.Contracts.IParser;
using System;
using Orchard.Localization;
using Orchard.UI.Notify;
using Transformalize;
using Transformalize.Desktop;
using Transformalize.Impl;
using Transformalize.Transform.DateMath;

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

            // javascript implementation
            builder.Register<ITransform>((ctx, p) => new JintTransform(p.TypedAs<PipelineContext>(), ctx.Resolve<IReader>())).Named<ITransform>("js");
            // razor implementation
            builder.Register<ITransform>((ctx, p) => {
                var c = p.TypedAs<PipelineContext>();
                if (ctx.IsRegistered<ITemplateProcessor>()) {
                    try {
                        var processor = ctx.Resolve<ITemplateProcessor>();
                        processor.Verify(c.Transform.Template);
                        return new OrchardRazorTransform(c, processor);
                    } catch (Exception ex) {
                        ctx.Resolve<INotifier>().Warning(T(ex.Message));
                        c.Warn(ex.Message);
                        return new NullTransform(c);
                    }
                }
                return new NullTransform(c);
            }).Named<ITransform>("razor");

            builder.Register((ctx, p) => {

                var dependencies = new List<IDependency> {
                    ctx.Resolve<IReader>(),
                    ctx.Resolve<ISerializer>(),
                    new DateMathModifier(),
                    new EnvironmentModifier(),
                    ctx.ResolveNamed<ICustomizer>("js"),
                    new IllegalCharacterValidator()
                };

                if (!string.IsNullOrEmpty(ctx.ResolveNamed<string>("sh"))) {
                    var shr = new ShorthandRoot(ctx.ResolveNamed<string>("sh"), ctx.ResolveNamed<IReader>("file"));
                    if (shr.Errors().Any()) {
                        var context = ctx.IsRegistered<IContext>() ? ctx.Resolve<IContext>() : new PipelineContext(ctx.IsRegistered<IPipelineLogger>() ? ctx.Resolve<IPipelineLogger>() : new OrchardLogger(), new Process { Name = "Error" }.WithDefaults());
                        foreach (var error in shr.Errors()) {
                            context.Error(error);
                        }
                        context.Error("Please fix you shorthand configuration.  No short-hand is being processed.");
                    } else {
                        dependencies.Add(new ShorthandCustomizer(shr, new[] { "fields", "calculated-fields" }, "t", "transforms", "method"));
                    }
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
