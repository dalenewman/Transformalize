#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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

using System.Linq;
using Autofac;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Desktop.Transforms;
using Pipeline.Ioc.Autofac.Modules;
using Pipeline.Logging.NLog;

namespace Pipeline.Command {
    public static class ProcessFactory {

        public static bool TryCreate(string cfg, string shorthand, out Process process) {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new RootModule(shorthand));
            builder.Register<IPipelineLogger>(c => new NLogPipelineLogger(SlugifyTransform.Slugify(cfg))).As<IPipelineLogger>().SingleInstance();
            builder.Register<IContext>(c => new PipelineContext(c.Resolve<IPipelineLogger>())).As<IContext>();

            using (var scope = builder.Build().BeginLifetimeScope()) {
                process = scope.Resolve<Process>(new NamedParameter("cfg", cfg));

                var context = scope.Resolve<IContext>();
                foreach (var warning in process.Warnings()) {
                    context.Warn(warning);
                }

                if (process.Errors().Any()) {
                    foreach (var error in process.Errors()) {
                        context.Error(error);
                    }
                    context.Error("The configuration errors must be fixed before this job will run.");
                } else {
                    process.Preserve = true;
                }
            }

            return process.Errors().Length == 0;
        }

        public static Process Create(string cfg, string shorthand) {
            Process process;
            TryCreate(cfg, shorthand, out process);
            return process;
        }
    }
}
