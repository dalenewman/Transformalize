#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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

using System.Linq;
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Reader;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;

namespace Transformalize.Providers.Razor.Autofac {
    public class RazorProviderModule : Module {

        private const string Razor = "razor";

        protected override void Load(ContainerBuilder builder) {

            if (!builder.Properties.ContainsKey("Process")) {
                return;
            }

            var process = (Process)builder.Properties["Process"];

            // template engines for template actions creating output with an action
            foreach (var t in process.Templates.Where(t => t.Enabled && t.Engine == Razor)) {
                var template = t;
                builder.Register<ITemplateEngine>(ctx => {
                    var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process);
                    context.Debug(() => $"Registering {template.Engine} Engine for {t.Key}");
                    return new RazorTemplateEngine(context, template, ctx.Resolve<IReader>());
                }).Named<ITemplateEngine>(t.Key);
            }

            // connections, no schema for razor
            foreach (var connection in process.Connections.Where(c => c.Provider == Razor)) {
                builder.Register<ISchemaReader>(ctx => new NullSchemaReader()).Named<ISchemaReader>(connection.Key);
            }

            // Entity input
            foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Input).Provider == Razor)) {

                // no input version detector
                builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

                // no input reader
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    return new NullReader(input, false);
                }).Named<IRead>(entity.Key);
            }

            if (process.GetOutputConnection().Provider == Razor) {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in process.Entities) {
                    builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

                    // ENTITY WRITER
                    builder.Register<IWrite>(ctx => new RazorWriter(ctx.ResolveNamed<OutputContext>(entity.Key), new DefaultReader(new FileReader(), new WebReader()))).Named<IWrite>(entity.Key);
                }
            }

        }
    }
}