#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Transformalize.Ioc.Autofac;
using Transformalize.Logging.NLog;
using Transformalize.Transforms.Globalization;

namespace Transformalize.Command {
    public static class ProcessFactory {

        public static bool TryCreate(Options options, Dictionary<string, string> parameters, out Process process) {

            var logger = new NLogPipelineLogger(SlugifyTransform.Slugify(options.Arrangement));
            using (var scope = ConfigurationContainer.Create(options.Arrangement, logger, parameters, options.PlaceHolderStyle)) {
                process = scope.Resolve<Process>();
                var context = new PipelineContext(logger, process);
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

            return process != null && process.Errors().Length == 0;

        }

        public static Process Create(Options options, Dictionary<string, string> parameters) {
            TryCreate(options, parameters, out var process);
            return process;
        }
    }
}
