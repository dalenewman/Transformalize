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
using Autofac;
using Humanizer;
using Humanizer.Bytes;
using Quartz;
using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Ioc.Autofac;
using Transformalize.Logging.NLog;
using Transformalize.Transforms.Globalization;
using Environment = System.Environment;

namespace Transformalize.Command {
   [DisallowConcurrentExecution]
   public class BaseExecutor : IRunTimeExecute {
      private readonly IPipelineLogger _logger;
      private readonly bool _checkMemory;

      public string Cfg { get; set; }
      public string Format { get; set; }
      public Options Options { get; }
      public static int RunCount { get; set; } = 1;
      public static ByteSize MaxBytes { get; set; }

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

         if (RunCount % 3 == 0) {
            if (_checkMemory) {
               CheckMemory(process, logger);
            } else {
               var context = new PipelineContext(logger, process);
               context.Info($"Process has run {RunCount} time{RunCount.Plural()}.");
            }
         }

         using (var scope = DefaultContainer.Create(process, logger, Options.PlaceHolderStyle)) {
            try {
               scope.Resolve<IProcessController>().Execute();
            } catch (Exception ex) {

               var context = scope.Resolve<IContext>();
               context.Error(ex.Message);
               context.Logger.Clear();

               new LibaryVersionChecker(context).Check();
            }
         }
         ++RunCount;
      }

      private static void CheckMemory(Process process, IPipelineLogger logger) {

         if (string.IsNullOrEmpty(process.MaxMemory))
            return;

         var context = new PipelineContext(logger, process);

         GC.Collect();
         GC.WaitForPendingFinalizers();

         var bytes = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64.Bytes();

         if (bytes.Megabytes > MaxBytes.Megabytes) {
            if (MaxBytes.Megabytes > 0.0) {
               context.Warn($"Process memory has increased from {MaxBytes.Megabytes:#.0} to {bytes.Megabytes:#.0}.");
            }
            MaxBytes = bytes;
         }

         var maxMemory = ByteSize.Parse(process.MaxMemory);

         if (maxMemory.CompareTo(bytes) < 0) {
            context.Error($"Process exceeded {maxMemory.Megabytes:#.0} Mb. Current memory is {bytes.Megabytes:#.0} Mb!");
            context.Error($"Process is exiting after running {RunCount} time{RunCount.Plural()}.");
            Environment.Exit(1);
         } else {
            context.Info($"Process is using {bytes.Megabytes:#.0} Mb of it's max {maxMemory.Megabytes:#.0} Mb allowed.");
            context.Info($"Process has run {RunCount} time{RunCount.Plural()}.");
         }
      }

      public void Execute(string cfg, Dictionary<string, string> parameters) {
         if (ProcessFactory.TryCreate(Options, parameters, out var process)) {
            process.Mode = Options.Mode;
            Execute(process);
         }
      }

   }
}