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
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Provider.Console;

namespace Transformalize.Ioc.Autofac.Modules {

    public class ConsoleModule : Module {
        private readonly Process _process;

        public ConsoleModule() { }

        public ConsoleModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // Connections
            foreach (var connection in _process.Connections.Where(c => c.Provider == "console")) {
                builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
            }

            // Entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "console")) {

                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                    return new ConsoleReader(input, rowFactory);
                }).Named<IRead>(entity.Key);

                builder.Register<IInputProvider>(ctx => new ConsoleInputProvider(ctx.ResolveNamed<IRead>(entity.Key))).Named<IInputProvider>(entity.Key);
            }

            // Entity Output
            var output = _process.Output();
            if (output.Provider == "console") {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in _process.Entities) {
                    builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

                    builder.Register<IWrite>(ctx => {
                        var serializer = output.Format == "json" ? new JsonNetSerializer(ctx.ResolveNamed<OutputContext>(entity.Key)) : new CsvSerializer(ctx.ResolveNamed<OutputContext>(entity.Key)) as ISerialize;
                        return new ConsoleWriter(serializer);
                    }).Named<IWrite>(entity.Key);

                    builder.Register<IOutputProvider>(ctx => {
                        var context = ctx.ResolveNamed<OutputContext>(entity.Key);
                        return new ConsoleOutputProvider(context, ctx.ResolveNamed<IWrite>(entity.Key));
                    })
                    .Named<IOutputProvider>(entity.Key);
                }
            }
        }
    }
}