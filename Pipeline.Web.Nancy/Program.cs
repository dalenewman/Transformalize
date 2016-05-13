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
using System.Threading;
using Autofac;
using Pipeline.Contracts;
using Pipeline.Logging.NLog;

namespace Pipeline.Web.Nancy {
    class Program {

        static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        static void Main(string[] args) {

            Console.CancelKeyPress += (sender, eArgs) => {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options)) {

                var container = new ContainerBuilder();
                container.Register(ctx => new NLogPipelineLogger("Pipeline.Service", options.LogLevel)).As<IPipelineLogger>().InstancePerLifetimeScope();
                container.RegisterModule(new HostModule());

                using (var scope = container.Build().BeginLifetimeScope()) {
                    try {
                        var host = scope.Resolve<IHost>(new NamedParameter("port", options.Port));
                        host.Start();

                        QuitEvent.WaitOne();
                        Console.WriteLine("Stopping...");
                        host.Stop();
                        Environment.ExitCode = 0;
                    } catch (Exception ex) {
                        Console.Error.WriteLine(ex.Message);
                        Environment.ExitCode = 1;
                    }
                }

            } else {
                Console.Error.WriteLine(options.GetUsage());
                Environment.ExitCode = 1;
            }


        }

    }
}
