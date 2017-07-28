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
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Provider.SSAS;
using Transformalize.Provider.Ado;

namespace Transformalize.Ioc.Autofac {
    public class SSASModule : Module {
        private readonly Process _process;

        public SSASModule() { }

        public SSASModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {
            if (_process == null)
                return;

            // connections
            foreach (var connection in _process.Connections.Where(c => c.Provider == "ssas")) {
                builder.Register<ISchemaReader>(ctx => {
                    return new NullSchemaReader();
                }).Named<ISchemaReader>(connection.Key);
            }

            // Entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "ssas")) {

                // input version detector
                builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

                // input reader
                builder.Register<IRead>(ctx => {
                    return new NullReader(ctx.ResolveNamed<InputContext>(entity.Key), false);
                }).Named<IRead>(entity.Key);
            }

            if (_process.Output().Provider == "ssas") {
                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in _process.Entities) {
                    builder.Register<IOutputController>(ctx => {
                        var input = ctx.ResolveNamed<InputContext>(entity.Key);
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var factory = ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key);
                        var initializer = _process.Mode == "init" ? (IAction)new SSASInitializer(input, output, factory) : new NullInitializer();
                        return new SSASOutputController(
                            output,
                            initializer,
                            ctx.ResolveNamed<IInputProvider>(entity.Key),
                            new SSASOutputProvider(input, output)
                        );
                    }
                    ).Named<IOutputController>(entity.Key);

                    // ENTITY WRITER
                    builder.Register<IWrite>(ctx => {
                        return new SSASWriter(
                            ctx.ResolveNamed<InputContext>(entity.Key), 
                            ctx.ResolveNamed<OutputContext>(entity.Key)
                        );
                    }).Named<IWrite>(entity.Key);
                }
            }

        }
    }
}