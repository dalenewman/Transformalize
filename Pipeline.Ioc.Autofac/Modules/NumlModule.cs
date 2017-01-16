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
using Transformalize.Nulls;
using Transformalize.Provider.GeoJson;
using Transformalize.Provider.Numl;

namespace Transformalize.Ioc.Autofac.Modules {
    public class NumlModule : Module {

        private const string Numl = "numl";

        private readonly Process _process;

        public NumlModule() { }

        public NumlModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // numl does not support input, nor reading it's schema
            foreach (var connection in _process.Connections.Where(c => c.Provider == Numl)) {
                builder.Register<ISchemaReader>(ctx => new NullSchemaReader()).Named<ISchemaReader>(connection.Key);
            }

            // numl input not supported
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == Numl)) {

                // input version detector
                builder.RegisterType<NullVersionDetector>().Named<IInputVersionDetector>(entity.Key);

                // input read
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    return new NullReader(input, false);
                }).Named<IRead>(entity.Key);

            }

            // If entity output not numl, leave
            if (_process.Output().Provider != Numl)
                return;


            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in _process.Entities) {

                // ENTITY OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

                // ENTITY WRITER
                builder.Register<IWrite>(ctx => {
                    var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                    switch (output.Connection.Provider) {
                        case Numl:
                            return new NumlModelWriter(output);
                        default:
                            return new NullWriter(output);
                    }
                }).Named<IWrite>(entity.Key);
            }
        }
    }
}