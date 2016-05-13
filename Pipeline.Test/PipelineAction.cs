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
using Autofac;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Desktop.Loggers;
using Pipeline.Ioc.Autofac.Modules;

namespace Pipeline.Test {
    public class PipelineAction : IAction {

        private readonly Process _process;
        private readonly IContext _context;

        public PipelineAction(Process process, IContext context = null) {
            _process = process;
            _context = context;
        }

        public ActionResponse Execute() {
            var response = new ActionResponse();
            if (!_process.Enabled) {
                response.Code = 503;
                response.Content = "Process is disabled.";
                return response;

            }
            var builder = new ContainerBuilder();

            // register
            builder.RegisterInstance(_context == null ? new TraceLogger() : _context.Logger);
            builder.RegisterModule(new RootModule("Shorthand.xml"));
            builder.RegisterModule(new ContextModule(_process));

            // providers
            builder.RegisterModule(new AdoModule(_process));
            builder.RegisterModule(new LuceneModule(_process));
            builder.RegisterModule(new SolrModule(_process));
            builder.RegisterModule(new ElasticModule(_process));
            builder.RegisterModule(new InternalModule(_process));
            builder.RegisterModule(new FileModule(_process));
            builder.RegisterModule(new FolderModule(_process));
            builder.RegisterModule(new ExcelModule(_process));

            builder.RegisterModule(new MapModule(_process));
            builder.RegisterModule(new TemplateModule(_process));
            builder.RegisterModule(new ActionModule(_process));

            builder.RegisterModule(new EntityPipelineModule(_process));
            builder.RegisterModule(new ProcessPipelineModule(_process));
            builder.RegisterModule(new ProcessControlModule(_process));

            using (var scope = builder.Build().BeginLifetimeScope()) {
                scope.ResolveNamed<IProcessController>(_process.Key).Execute();
            }

            return response;
        }
    }
}
