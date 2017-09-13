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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac.Modules;
using Transformalize.Logging.NLog;
using Transformalize.Transforms.Globalization;

namespace Transformalize.Command {
    public static class ProcessFactory {

        public static bool TryCreate(string cfg, Dictionary<string,string> parameters, out Process process) {

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ShorthandModule("t"));
            builder.RegisterModule(new ShorthandModule("v"));
            builder.RegisterModule(new RootModule());
            builder.Register<IPipelineLogger>(c => new NLogPipelineLogger(SlugifyTransform.Slugify(cfg))).As<IPipelineLogger>().SingleInstance();
            builder.Register<IContext>(c => new PipelineContext(c.Resolve<IPipelineLogger>())).As<IContext>();

            using (var scope = builder.Build().BeginLifetimeScope()) {
                process = scope.Resolve<Process>(new NamedParameter("cfg", cfg), new NamedParameter("parameters", parameters));

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

        public static Process Create(string cfg, Dictionary<string,string> parameters) {
            Process process;
            TryCreate(cfg, parameters, out process);
            return process;
        }
    }
}
