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
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Ioc.Autofac.Modules;

namespace Pipeline.Ioc.Autofac {

    public class RunTimeExecutor : IRunTimeExecute {
        private readonly IContext _context;

        public RunTimeExecutor(IContext context) {
            _context = context;
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

            var builder = new ContainerBuilder();
            builder.RegisterInstance(_context.Logger).As<IPipelineLogger>().SingleInstance();
            builder.RegisterCallback(new RootModule("Shorthand.xml").Configure);
            builder.RegisterCallback(new ContextModule(process).Configure);

            // providers
            builder.RegisterCallback(new AdoModule(process).Configure);
            builder.RegisterCallback(new LuceneModule(process).Configure);
            builder.RegisterCallback(new SolrModule(process).Configure);
            builder.RegisterCallback(new ElasticModule(process).Configure);
            builder.RegisterCallback(new InternalModule(process).Configure);
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
                    scope.ResolveNamed<IProcessController>(process.Key).Execute();
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

    }
}