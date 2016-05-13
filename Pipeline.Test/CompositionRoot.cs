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
using System;
using System.Linq;
using Pipeline.Contracts;
using Pipeline.Desktop.Loggers;
using Pipeline.Ioc.Autofac.Modules;

namespace Pipeline.Test {
    public class CompositionRoot {

        public Process Process { get; set; }

        public IProcessController Compose(string cfg, LogLevel logLevel = LogLevel.Debug) {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new RootModule(@"Files\Shorthand.xml"));
            var container = builder.Build();

            Process = container.Resolve<Process>(new NamedParameter("cfg", cfg));

            if (Process.Errors().Any()) {
                foreach (var error in Process.Errors()) {
                    System.Diagnostics.Trace.WriteLine(error);
                }
                throw new Exception("Configuration Error(s)");
            }

            if (Process.Warnings().Any()) {
                foreach (var warning in Process.Warnings()) {
                    System.Diagnostics.Trace.WriteLine(warning);
                }
            }

            builder = new ContainerBuilder();
            builder.Register<IPipelineLogger>(ctx => new TraceLogger(logLevel)).SingleInstance();
            builder.RegisterModule(new RootModule(@"Files\Shorthand.xml"));
            builder.RegisterModule(new ContextModule(Process));

            // providers
            builder.RegisterModule(new AdoModule(Process));
            builder.RegisterModule(new LuceneModule(Process));
            builder.RegisterModule(new SolrModule(Process));
            builder.RegisterModule(new ElasticModule(Process));
            builder.RegisterModule(new InternalModule(Process));
            builder.RegisterModule(new FileModule(Process));
            builder.RegisterModule(new FolderModule(Process));
            builder.RegisterModule(new ExcelModule(Process));

            builder.RegisterModule(new MapModule(Process));
            builder.RegisterModule(new TemplateModule(Process));
            builder.RegisterModule(new ActionModule(Process));

            builder.RegisterModule(new EntityPipelineModule(Process));
            builder.RegisterModule(new ProcessPipelineModule(Process));
            builder.RegisterModule(new ProcessControlModule(Process));

            container = builder.Build();

            return container.ResolveNamed<IProcessController>(Process.Key);
        }

    }

}
