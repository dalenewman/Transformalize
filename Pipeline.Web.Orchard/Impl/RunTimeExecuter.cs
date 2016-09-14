#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
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
using Orchard.FileSystems.AppData;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Modules;

namespace Pipeline.Web.Orchard.Impl {
    public class RunTimeExecuter : IRunTimeExecute {
        private readonly IContext _context;
        private readonly IAppDataFolder _appDataFolder;

        public RunTimeExecuter(IContext context, IAppDataFolder appDataFolder) {
            _appDataFolder = appDataFolder;
            _context = context;
        }

        public void Execute(Process process) {
            if (!process.Enabled) {
                _context.Error("Process is disabled");
                return;
            }

            foreach (var warning in process.Warnings()) {
                _context.Warn(warning);
            }

            if (process.Errors().Any()) {
                foreach (var error in process.Errors()) {
                    _context.Error(error);
                }
                _context.Error("The configuration errors must be fixed before this job will execute.");
                return;
            }

            var container = new ContainerBuilder();
            container.RegisterInstance(_context.Logger).As<IPipelineLogger>().SingleInstance();
            container.RegisterCallback(new RootModule().Configure);
            container.RegisterCallback(new ContextModule(process).Configure);

            // providers
            container.RegisterCallback(new AdoModule(process).Configure);
            container.RegisterCallback(new SolrModule(process).Configure);
            container.RegisterCallback(new ElasticModule(process).Configure);
            container.RegisterCallback(new InternalModule(process).Configure);
            container.RegisterCallback(new FileModule(process, _appDataFolder).Configure);
            container.RegisterCallback(new ExcelModule(process, _appDataFolder).Configure);
            container.RegisterCallback(new WebModule(process).Configure);

            container.RegisterCallback(new MapModule(process).Configure);
            container.RegisterCallback(new ActionModule(process).Configure);

            container.RegisterCallback(new EntityPipelineModule(process).Configure);
            container.RegisterCallback(new ProcessPipelineModule(process).Configure);
            container.RegisterCallback(new ProcessControlModule(process).Configure);

            using (var scope = container.Build().BeginLifetimeScope()) {
                var logger = scope.Resolve<IPipelineLogger>() as OrchardLogger;
                if (logger != null) {
                    logger.Process = process;
                }
                scope.Resolve<IProcessController>().Execute();
            }

        }

        public void Execute(string cfg, string shorthand, Dictionary<string, string> parameters) {
            var container = new ContainerBuilder();
            container.RegisterInstance(_context.Logger).As<IPipelineLogger>().SingleInstance();
            container.RegisterCallback(new RootModule().Configure);

            var format = parameters.ContainsKey("format") ? parameters["format"] : "xml";
            using (var scope = container.Build().BeginLifetimeScope()) {
                var process = format == "json" ? scope.Resolve<JsonProcess>() : scope.Resolve<XmlProcess>() as Process;
                Execute(process);
            }
        }
    }
}