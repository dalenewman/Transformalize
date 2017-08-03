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
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Humanizer;
using Humanizer.Bytes;
using Quartz;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Desktop.Transforms;
using Transformalize.Ioc.Autofac;
using Transformalize.Logging.NLog;
using Environment = System.Environment;

namespace Transformalize.Command {
    [DisallowConcurrentExecution]
    public class BaseExecutor : IRunTimeExecute {
        private readonly IPipelineLogger _logger;
        private readonly bool _checkMemory;

        public string Cfg { get; set; }
        public string Format { get; set; }
        public Options Options { get; }

        public BaseExecutor(IPipelineLogger logger, Options options, bool checkMemory) {
            _logger = logger;
            Options = options;
            _checkMemory = checkMemory;
        }

        public void Execute(Process process) {
            var logger = _logger ?? new NLogPipelineLogger(SlugifyTransform.Slugify(Cfg));

            if (process.OutputIsConsole()) {
                logger.SuppressConsole();
            }

            if (_checkMemory) {
                CheckMemory(process, logger);
            }

            using (var scope = DefaultContainer.Create(process, logger, Options.PlaceHolderStyle)) {
                try {
                    scope.Resolve<IProcessController>().Execute();
                } catch (Exception ex) {
                    var context = scope.Resolve<IContext>();
                    context.Error(ex.Message);
                    context.Logger.Clear();
                }
            }
        }

        private static void CheckMemory(Process process, IPipelineLogger logger) {
            if (string.IsNullOrEmpty(process.MaxMemory))
                return;

            var context = new PipelineContext(logger, process);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var currentBytes = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64.Bytes();
            var maxMemory = ByteSize.Parse(process.MaxMemory);

            if (maxMemory.CompareTo(currentBytes) < 0) {
                context.Error(
                    $"Process exceeded {maxMemory.Megabytes:#.0} Mb. Current memory is {currentBytes.Megabytes:#.0} Mb!");
                Environment.Exit(1);
            } else {
                context.Info(
                    $"The process is using {currentBytes.Megabytes:#.0} Mb of it's max {maxMemory.Megabytes:#.0} Mb allowed.");
            }
        }

        public void Execute(string cfg, Dictionary<string, string> parameters) {
            Process process;
            if (ProcessFactory.TryCreate(cfg, parameters, out process)) {
                process.Mode = Options.Mode;
                Execute(process);
            }
        }

    }
}