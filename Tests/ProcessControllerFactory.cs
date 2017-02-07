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
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Desktop.Loggers;
using Transformalize.Ioc.Autofac.Modules;

namespace Tests {
    public static class ProcessControllerFactory {
        public static IProcessController Create(Process process, IPipelineLogger logger = null) {
            var builder = new ContainerBuilder();
            builder.Register(ctx => logger ?? new TraceLogger()).SingleInstance();
            builder.RegisterModule(new RootModule(@"Files\Shorthand.xml"));
            builder.RegisterModule(new ContextModule(process));

            // providers
            builder.RegisterModule(new AdoModule(process));
            builder.RegisterModule(new LuceneModule(process));
            builder.RegisterModule(new SolrModule(process));
            builder.RegisterModule(new ElasticModule(process));
            builder.RegisterModule(new InternalModule(process));
            builder.RegisterModule(new FileModule(process));
            builder.RegisterModule(new GeoJsonModule(process));
            builder.RegisterModule(new FolderModule(process));
            builder.RegisterCallback(new DirectoryModule(process).Configure);
            builder.RegisterModule(new ExcelModule(process));
            builder.RegisterModule(new WebModule(process));

            builder.RegisterModule(new MapModule(process));
            builder.RegisterModule(new TemplateModule(process));
            builder.RegisterModule(new ActionModule(process));

            builder.RegisterModule(new EntityPipelineModule(process));
            builder.RegisterModule(new ProcessPipelineModule(process));
            builder.RegisterModule(new ProcessControlModule(process));

            var container = builder.Build();

            return container.Resolve<IProcessController>();
        }
    }
}