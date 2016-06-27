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
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Environment;
using Cfg.Net.Reader;
using Cfg.Net.Serializers;
using Cfg.Net.Shorthand;
using Pipeline.Configuration;
using Pipeline.Nulls;
using Pipeline.Scripting.Jint;

namespace Pipeline.Ioc.Autofac.Modules {

    public class UnloadedRootModule : Module {
        private readonly string _shorthand;
        private readonly string _format;

        public UnloadedRootModule() { }

        public UnloadedRootModule(string shorthand, string format = null) {
            _shorthand = shorthand;
            _format = format ?? "json";
        }

        protected override void Load(ContainerBuilder builder) {

            builder.RegisterType<JintValidator>().Named<IValidator>("js");
            builder.RegisterType<FileReader>().Named<IReader>("file");
            builder.RegisterType<WebReader>().Named<IReader>("web");

            builder.Register(ctx => new EnvironmentModifier(
                new PlaceHolderModifier(), 
                new ParameterModifier())
            ).As<IRootModifier>();

            if (_format == "xml") {
                builder.RegisterType<XmlSerializer>().As<ISerializer>();
            } else {
                builder.RegisterType<JsonSerializer>().As<ISerializer>();
            }

            builder.Register<IReader>(ctx => new DefaultReader(
                ctx.ResolveNamed<IReader>("file"),
                new ReTryingReader(ctx.ResolveNamed<IReader>("web"), attempts: 3))
            );

            builder.Register((ctx, p) => {
                var dependencies = new List<IDependency> {
                    ctx.Resolve<IReader>(),
                    ctx.Resolve<ISerializer>(),
                    new PlaceHolderModifier(),
                    ctx.Resolve<IRootModifier>(),
                    ctx.ResolveNamed<IValidator>("js"),
                    new PlaceHolderValidator()
                };

                if (!string.IsNullOrEmpty(_shorthand)) {
                    var shr = new ShorthandRoot(_shorthand, ctx.ResolveNamed<IReader>("file"));
                    dependencies.Add(new ShorthandValidator(shr, "sh"));
                    dependencies.Add(new ShorthandModifier(shr, "sh"));
                } else {
                    dependencies.Add(new NullValidator("sh"));
                }

                return new Process(dependencies.ToArray());

        }
    }
}
