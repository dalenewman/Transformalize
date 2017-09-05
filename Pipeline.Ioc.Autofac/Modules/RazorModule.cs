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
using Cfg.Net.Reader;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Providers.Razor;

namespace Transformalize.Ioc.Autofac.Modules {
    public class RazorModule : Module {
        private readonly Process _process;
        private const string Razor = "razor";
        public RazorModule() { }

        public RazorModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {
            if (_process == null)
                return;

            // connections, no schema for razor
            foreach (var connection in _process.Connections.Where(c => c.Provider == Razor)) {
                builder.Register<ISchemaReader>(ctx => new NullSchemaReader()).Named<ISchemaReader>(connection.Key);
            }

            // Entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == Razor)) {

                // no input version detector
                builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

                // no input reader
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    return new NullReader(input, false);
                }).Named<IRead>(entity.Key);
            }

            if (_process.Output().Provider == Razor) {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in _process.Entities) {
                    builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

                    // ENTITY WRITER
                    builder.Register<IWrite>(ctx => new RazorWriter(ctx.ResolveNamed<OutputContext>(entity.Key), new DefaultReader(new FileReader(), new WebReader()))).Named<IWrite>(entity.Key);
                }
            }

        }
    }
}