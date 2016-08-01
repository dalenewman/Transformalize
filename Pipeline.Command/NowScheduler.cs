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
using System.Linq;
using Autofac;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Ioc.Autofac.Modules;
using Pipeline.Logging.NLog;

namespace Pipeline.Command {
    public class NowScheduler : IScheduler {

        private readonly Options _options;
        private readonly ISchemaHelper _schemaHelper;

        public NowScheduler(Options options, ISchemaHelper schemaHelper) {
            _options = options;
            _schemaHelper = schemaHelper;
        }

        public void Start() {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new RootModule(_options.Shorthand));
            builder.Register<IPipelineLogger>(c => new NLogPipelineLogger(_options.Arrangement)).As<IPipelineLogger>().SingleInstance();
            builder.Register<IContext>(c => new PipelineContext(c.Resolve<IPipelineLogger>())).As<IContext>();
            builder.Register(c=>new RunTimeExecutor()).As<IRunTimeExecute>();

            using (var scope = builder.Build().BeginLifetimeScope()) {
                var context = scope.Resolve<IContext>();
                var process = scope.Resolve<Configuration.Process>(
                    new NamedParameter("cfg", _options.Arrangement)
                );
                foreach (var warning in process.Warnings()) {
                    context.Warn(warning);
                }

                if (process.Errors().Any()) {
                    foreach (var error in process.Errors()) {
                        context.Error(error);
                    }
                    context.Error("The configuration errors must be fixed before this job will run.");
                    return;
                }

                if (process.Entities.Any(e => !e.Fields.Any(f => f.Input))) {
                    context.Info("Detecting schema...");
                    if (_schemaHelper.Help(process)) {
                        if (process.Errors().Any()) {
                            foreach (var error in process.Errors()) {
                                context.Error(error);
                            }
                            context.Error("The configuration errors must be fixed before this job will run.");
                            return;
                        }
                    }
                }

                if (_options.Mode != null && _options.Mode.ToLower() == "check") {
                    foreach (var entity in process.Entities) {
                        if (entity.Name == entity.Alias) {
                            entity.Alias = null;
                        }
                        entity.Fields.RemoveAll(f => f.System);
                    }
                    foreach (var field in process.GetAllFields().Where(f => !string.IsNullOrEmpty(f.T))) {
                        field.T = string.Empty;
                    }
                    foreach (var field in process.GetAllFields()) {
                        if (field.Name == field.Alias) {
                            field.Alias = null;
                        }
                        if (field.Name == field.Label) {
                            field.Label = string.Empty;
                        }
                    }
                    Console.WriteLine(process.Serialize());
                    return;
                }

                scope.Resolve<IRunTimeExecute>().Execute(process);
            }

        }

        public void Stop() {
        }
    }
}