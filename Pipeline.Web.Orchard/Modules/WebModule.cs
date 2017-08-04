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

namespace Pipeline.Web.Orchard.Modules {

    public class WebModule : Module {

        private readonly Process _process;

        public WebModule() { }

        public WebModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // Connections
            foreach (var connection in _process.Connections.Where(c => c.Provider == "web")) {
                // Schema Reader
                builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
            }

            // entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "web")) {

                // input version detector
                builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

                // input read
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

                    switch (input.Connection.Provider) {
                        case "web":
                            if (input.Connection.Delimiter == string.Empty && input.Entity.Fields.Count(f => f.Input) == 1) {
                                return new Transformalize.Provider.Web.WebReader(input, rowFactory);
                            }
                            return new Transformalize.Provider.Web.WebCsvReader(input, rowFactory);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);

            }

            // TODO: be able to post (write) to web
            if (_process.Output().Provider == "web") {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in _process.Entities) {

                    // ENTITY OUTPUT CONTROLLER
                    builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

                    // ENTITY WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        return new NullWriter(output);
                    }).Named<IWrite>(entity.Key);

                }
            }

        }
    }
}