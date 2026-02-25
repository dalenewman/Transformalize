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

using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Providers.OpenXml;

namespace Transformalize.Providers.Excel.Autofac {
    public class ExcelModule : Module {

        protected override void Load(ContainerBuilder builder) {

            if (!builder.Properties.ContainsKey("Process")) {
                return;
            }

            var p = (Process)builder.Properties["Process"];

            var schemaReaders = new HashSet<string>();

            // Entity input
            foreach (var entity in p.Entities.Where(e => p.Connections.First(c => c.Name == e.Input).Provider == "excel")) {

                var connection = p.Connections.First(c => c.Name == entity.Input);

                if (schemaReaders.Add(connection.Key)) {
                    builder.Register<ISchemaReader>(ctx => {
                        
                        /* file and excel are different, have to load the content and check it to determine schema */
                        var fileInfo = new FileInfo(Path.IsPathRooted(connection.File) ? connection.File : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, connection.File));
                        var context = new InputContext(new PipelineContext(ctx.Resolve<IPipelineLogger>(), p, entity));
                        var cfg = new ExcelInspection(context, fileInfo, 100).Create();
                        var process = new Process(cfg);

                        foreach (var warning in process.Warnings()) {
                            context.Warn(warning);
                        }

                        if (process.Errors().Any()) {
                            foreach (var error in process.Errors()) {
                                context.Error(error);
                            }
                            return new NullSchemaReader();
                        }

                        return new ExcelSchemaReader(process, new InputContext(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, process.Entities.First())));

                    }).Named<ISchemaReader>(connection.Key);
                }

                // input version detector
                builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

                // input reader
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                    switch (input.Connection.Provider) {
                        case "excel":
                            return new ExcelReader(input, rowFactory);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);
            }

            if (p.GetOutputConnection().Provider == "excel") {
                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in p.Entities) {
                    builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

                    // ENTITY WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                        switch (output.Connection.Provider) {
                            case "excel":
                                return new ExcelWriter(output);
                            default:
                                return new NullWriter(output);
                        }
                    }).Named<IWrite>(entity.Key);
                }
            }

        }
    }
}