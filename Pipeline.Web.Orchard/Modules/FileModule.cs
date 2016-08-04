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
using System;
using System.IO;
using System.Linq;
using Autofac;
using Orchard.FileSystems.AppData;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Desktop;
using Pipeline.Nulls;
using Pipeline.Provider.File;
using Pipeline.Web.Orchard.Impl;

namespace Pipeline.Web.Orchard.Modules {
    public class FileModule : Module {
        private readonly Process _process;
        private readonly IAppDataFolder _appDataFolder;

        public FileModule() { }

        public FileModule(Process process, IAppDataFolder appDataFolder) {
            _process = process;
            _appDataFolder = appDataFolder;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // Connections
            foreach (var connection in _process.Connections.Where(c => c.Provider == "file")) {

                // Schema Reader
                builder.Register<ISchemaReader>(ctx => {
                    /* file and excel are different, have to load the content and check it to determine schema */
                    var fileInfo = new FileInfo(Path.IsPathRooted(connection.File) ? connection.File : _appDataFolder.Combine(Common.FileFolder, connection.File));
                    var context = ctx.ResolveNamed<IConnectionContext>(connection.Key);
                    var cfg = new FileInspection(context, fileInfo, 100).Create();
                    var process = ctx.Resolve<Process>();
                    process.Load(cfg);

                    foreach (var warning in process.Warnings()) {
                        context.Warn(warning);
                    }

                    if (process.Errors().Any()) {
                        foreach (var error in process.Errors()) {
                            context.Error(error);
                        }
                        return new NullSchemaReader();
                    }

                    return new SchemaReader(context, new RunTimeRunner(context, _appDataFolder), process);

                }).Named<ISchemaReader>(connection.Key);
            }

            // entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "file")) {

                // input version detector
                builder.RegisterType<NullVersionDetector>().Named<IInputVersionDetector>(entity.Key);

                // input read
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

                    switch (input.Connection.Provider) {
                        case "file":

                            if (input.Connection.Delimiter == string.Empty &&
                                input.Entity.Fields.Count(f => f.Input) == 1) {
                                return new FileReader(input, rowFactory);
                            }

                            IRowCondition condition = new NullRowCondition();
                            if (input.Entity.Filter.Any()) {
                                var expression = input.Entity.Filter.First().Expression;
                                if (ctx.IsRegisteredWithName<IParser>("js") && ctx.ResolveNamed<IParser>("js").Parse(expression, input.Error)) {
                                    condition = ctx.Resolve<IRowCondition>(new TypedParameter(typeof(InputContext), input), new TypedParameter(typeof(string), input.Entity.Filter.First().Expression));
                                } else {
                                    input.Error("Your {0} script is not parsing or registered. It is not filtering.", input.Entity.Alias);
                                }
                            }
                            return new DelimitedFileReader(input, rowFactory, condition);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);

            }

            // Entity Output
            if (_process.Output().Provider == "file") {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var e in _process.Entities) {
                    var entity = e;
                    // WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                        switch (output.Connection.Provider) {
                            case "file":
                                return new DelimitedFileWriter(output, output.Connection.File);
                            default:
                                return new NullWriter(output);
                        }
                    }).Named<IWrite>(entity.Key);

                }
            }

        }
    }
}