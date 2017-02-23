#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Transformalize.Extensions;
using Transformalize.Nulls;
using Pipeline.Web.Orchard.Impl;
using Transformalize;

namespace Pipeline.Web.Orchard.Modules {

    public class InternalModule : Module {

        private readonly Process _process;
        private readonly string[] _internal = { "internal", "console", "trace", "log" };

        public InternalModule() { }

        public InternalModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // Connections
            foreach (var connection in _process.Connections.Where(c => c.IsInternal())) {
                builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
            }

            // Entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).IsInternal())) {

                var e = entity;

                builder.RegisterType<NullVersionDetector>().Named<IInputVersionDetector>(e.Key);

                // READER
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(e.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(e.Key, new NamedParameter("capacity", input.RowCapacity));

                    switch (input.Connection.Provider) {
                        case "internal":
                            return new InternalReader(input, rowFactory);
                        case "console":
                            // todo: take standard input
                            return new NullReader(input);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);

            }

            // Entity Output
            if (_process.Output().Provider.In(_internal)) {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in _process.Entities) {

                    var e = entity;

                    builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

                    // WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(e.Key);

                        switch (output.Connection.Provider) {
                            case "internal":
                                return new InternalWriter(output);
                            case "log":
                                return new OrchardLogWriter(output);
                            default:
                                return new NullWriter(output);
                        }
                    }).Named<IWrite>(entity.Key);
                }
            }
        }
    }
}