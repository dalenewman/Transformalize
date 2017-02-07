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
using Transformalize.Provider.GeoJson;

namespace Transformalize.Ioc.Autofac.Modules {
    public class GeoJsonModule : Module {
        private readonly Process _process;

        public GeoJsonModule() { }

        public GeoJsonModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // geoJson schema reading not supported yet
            foreach (var connection in _process.Connections.Where(c => c.Provider == "geojson")) {
                builder.Register<ISchemaReader>(ctx => new NullSchemaReader()).Named<ISchemaReader>(connection.Key);
            }

            // geoJson input not supported yet
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "geojson")) {

                // input version detector
                builder.RegisterType<NullVersionDetector>().Named<IInputVersionDetector>(entity.Key);

                // input read
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    return new NullReader(input, false);
                }).Named<IRead>(entity.Key);

            }

            // If entity output not geoJson, leave
            if (_process.Output().Provider != "geojson")
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
                        case "geojson":
                            return new GeoJsonFileWriter(output);
                        default:
                            return new NullWriter(output);
                    }
                }).Named<IWrite>(entity.Key);
            }
        }
    }
}