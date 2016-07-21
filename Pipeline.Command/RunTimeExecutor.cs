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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Ioc.Autofac.Modules;
using Quartz;

namespace Pipeline.Command {

    [DisallowConcurrentExecution]
    public class RunTimeExecutor : IRunTimeExecute, IJob {
        private readonly Options _options;
        private readonly IContext _context;
        private readonly ISchemaHelper _schemaHelper;

        public RunTimeExecutor(Options options, IContext context, ISchemaHelper schemaHelper) {
            _options = options;
            _context = context;
            _schemaHelper = schemaHelper;
        }

        public void Execute(Process process) {

            foreach (var warning in process.Warnings()) {
                _context.Warn(warning);
            }

            if (process.Errors().Any()) {
                foreach (var error in process.Errors()) {
                    _context.Error(error);
                }
                _context.Error("The configuration errors must be fixed before this job will run.");
                return;
            }

            if (process.Entities.Any(e => !e.Fields.Any(f => f.Input))) {
                if (_schemaHelper.Help(process)) {
                    if (process.Errors().Any()) {
                        foreach (var error in process.Errors()) {
                            _context.Error(error);
                        }
                        _context.Error("The configuration errors must be fixed before this job will run.");
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
            } else {
                if (_options.Mode != null) {
                    process.Mode = _options.Mode;
                }
            }

            // Since we're in a Console app
            if (process.Output().IsInternal()) {
                process.Output().Provider = "console";
            }

            var builder = new ContainerBuilder();
            builder.RegisterInstance(_context.Logger).As<IPipelineLogger>().SingleInstance();
            builder.RegisterCallback(new RootModule("Shorthand.xml").Configure);
            builder.RegisterCallback(new ContextModule(process).Configure);

            // providers
            builder.RegisterCallback(new AdoModule(process).Configure);
            builder.RegisterCallback(new LuceneModule(process).Configure);
            builder.RegisterCallback(new SolrModule(process).Configure);
            builder.RegisterCallback(new ElasticModule(process).Configure);
            builder.RegisterCallback(new InternalModule(process, _options.Output).Configure);
            builder.RegisterCallback(new FileModule(process).Configure);
            builder.RegisterCallback(new FolderModule(process).Configure);
            builder.RegisterCallback(new ExcelModule(process).Configure);

            builder.RegisterCallback(new MapModule(process).Configure);
            builder.RegisterCallback(new TemplateModule(process).Configure);
            builder.RegisterCallback(new ActionModule(process).Configure);

            builder.RegisterCallback(new EntityPipelineModule(process).Configure);
            builder.RegisterCallback(new ProcessPipelineModule(process).Configure);
            builder.RegisterCallback(new ProcessControlModule(process).Configure);

            using (var scope = builder.Build().BeginLifetimeScope()) {
                try {
                    scope.Resolve<IProcessController>().Execute();
                } catch (Exception ex) {
                    _context.Error(ex.Message);
                }
            }
        }

        public void Execute(string cfg, string shorthand, Dictionary<string, string> parameters) {
            var builder = new ContainerBuilder();
            builder.RegisterCallback(new RootModule(shorthand).Configure);
            using (var scope = builder.Build().BeginLifetimeScope()) {
                var process = scope.Resolve<Process>(
                    new NamedParameter("cfg", cfg),
                    new NamedParameter("parameters", parameters)
                );
                Execute(process);
            }
        }

        public void Execute(IJobExecutionContext context) {
            Execute(_options.Arrangement, _options.Shorthand, null);
        }

    }
}