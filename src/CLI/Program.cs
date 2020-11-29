using Autofac;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Containers.Autofac.Modules;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.Console.Autofac;
using Transformalize.Providers.CsvHelper.Autofac;
using Transformalize.Providers.Json.Autofac;
using Transformalize.Providers.PostgreSql.Autofac;
using Transformalize.Providers.SqlServer.Autofac;

namespace Transformalize.Cli {
   class Program {
      static void Main(string[] args) {

         Parser.Default.ParseArguments<RunOptions>(args)
          .WithParsed(Run)
          .WithNotParsed(CommandLineError);

      }

      static void Run(RunOptions options) {


         var logger = new ConsoleLogger(options.LogLevel);

         using (var outer = new ConfigurationContainer().CreateScope(options.ArrangementWithMode(), logger, options.GetParameters())) {

            var process = outer.Resolve<Process>();

            if (process.Errors().Any()) {
               Environment.Exit(1);
            }

            var modules = new List<Autofac.Core.IModule> {
               new ConsoleProviderModule()
            };

            var output = process.GetOutputConnection();

            if(output == null || output.Provider == "internal" || output.Provider == "console") {
               logger.SuppressConsole();
               if(options.Format == "csv") {
                  output.Provider = "file";  // delimited file
                  output.Delimiter = ",";
                  output.Stream = true;
                  output.File = "dummy.csv";
                  modules.Add(new CsvHelperProviderModule(System.Console.OpenStandardOutput()));
               } else {
                  output.Provider = "json";
                  output.Stream = true;
                  output.Format = "json";
                  output.File = "dummy.json";
                  modules.Add(new JsonProviderModule(System.Console.OpenStandardOutput()));
               }
            } else {
               modules.Add(new CsvHelperProviderModule());
               modules.Add(new JsonProviderModule());
            }

            var providers = new HashSet<string>(process.Connections.Select(c => c.Provider));

            if (providers.Contains("sqlserver")) { modules.Add(new SqlServerModule()); }
            if (providers.Contains("postgresql")) { modules.Add(new PostgreSqlModule()); }
            if (providers.Contains("bogus")) { modules.Add(new BogusModule()); }

            using (var inner = new Container(modules.ToArray()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }
      static void CommandLineError(IEnumerable<Error> errors) {
         System.Environment.Exit(1);
      }


   }
}
