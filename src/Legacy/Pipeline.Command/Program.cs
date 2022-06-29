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
using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using CommandLine;
using Transformalize.Contracts;

namespace Transformalize.Command {
   internal class Program {

      private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

      private static void Main(string[] args) {

         Console.CancelKeyPress += (sender, eArgs) => {
            QuitEvent.Set();
            eArgs.Cancel = true;
         };

         Parser.Default.ParseArguments<Options>(args)
            .WithParsed(Run)
            .WithNotParsed(CommandLineError);
         NLog.LogManager.Flush();
      }

      static void Run(Options options) {

         Environment.ExitCode = 0;
         var builder = new ContainerBuilder();
         builder.RegisterModule(new ScheduleModule(options));

         using (var scope = builder.Build().BeginLifetimeScope()) {
            var scheduler = scope.Resolve<IScheduler>();
            scheduler.Start();

            if (scheduler is NowScheduler)
               return;

            QuitEvent.WaitOne();
            Console.WriteLine("Stopping...");
            scheduler.Stop();
         }

      }

      static void CommandLineError(IEnumerable<Error> errors) {
         Environment.Exit(1);
      }
   }
}
