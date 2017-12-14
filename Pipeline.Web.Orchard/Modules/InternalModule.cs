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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Pipeline.Web.Orchard.Impl;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Provider.Internal;
using Transformalize.Providers.File;

namespace Pipeline.Web.Orchard.Modules {

    public class InternalModule : Module {
        private readonly Process _process;
        private readonly HashSet<string> _internal = new HashSet<string>(new[] { "internal", "trace", "log", "text", Constants.DefaultSetting });

        public InternalModule() { }

        public InternalModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // Connections
            foreach (var connection in _process.Connections.Where(c => c.Provider == "internal")) {
                builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
            }

            // Entity input
            foreach (var entity in _process.Entities.Where(e => _internal.Contains(_process.Connections.First(c => c.Name == e.Connection).Provider))) {

                builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

                // READER
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

                    if (_process.Mode == "form") {
                        // if key filter exists
                        if (entity.GetPrimaryKey().Any() && entity.GetPrimaryKey().All(f => entity.Filter.Any(i => i.Field == f.Alias || i.Field == f.Name))) {
                            if (entity.GetPrimaryKey().All(pk => entity.Filter.First(i => i.Field == pk.Alias || i.Field == pk.Name).Value == (pk.Default == Constants.DefaultSetting ? Constants.StringDefaults()[pk.Type] : pk.Default))) {
                                // this is a form insert, create a new default row and apply parameters
                                return new ParameterRowReader(input, new DefaultRowReader(input, rowFactory));
                            }
                        }
                    }

                    switch (input.Connection.Provider) {
                        case Constants.DefaultSetting:
                        case "internal":
                            return new InternalReader(input, rowFactory);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);

            }

            // Entity Output
            if (_internal.Contains(_process.Output().Provider)) {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in _process.Entities) {

                    var e = entity;

                    builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(e.Key);
                    builder.Register<IOutputProvider>(ctx => new InternalOutputProvider(ctx.ResolveNamed<OutputContext>(e.Key), ctx.ResolveNamed<IWrite>(e.Key))).Named<IOutputProvider>(e.Key);

                    // WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(e.Key);

                        switch (output.Connection.Provider) {
                            case Constants.DefaultSetting:
                            case "internal":
                                return new InternalWriter(output);
                            case "text":
                                return new FileStreamWriter(output, HttpContext.Current.Response.OutputStream);
                            case "trace":
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