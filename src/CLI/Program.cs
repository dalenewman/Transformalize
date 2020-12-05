using Autofac;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.Console.Autofac;
using Transformalize.Providers.CsvHelper.Autofac;
using Transformalize.Providers.Elasticsearch.Autofac;
using Transformalize.Providers.Json.Autofac;
using Transformalize.Providers.PostgreSql.Autofac;
using Transformalize.Providers.Sqlite.Autofac;
using Transformalize.Providers.SqlServer.Autofac;
using Transformalize.Transforms.Jint.Autofac;

namespace Transformalize.Cli {
   class Program {
      static void Main(string[] args) {

         Parser.Default.ParseArguments<RunOptions>(args)
          .WithParsed(Run)
          .WithNotParsed(CommandLineError);

      }

      static void Run(RunOptions options) {


         var logger = new ConsoleLogger(options.LogLevel);

         var operations = new List<Autofac.Core.IModule> {
            new JintTransformModule()
         };
         // razor, lambda parser, fluid, humanizer, etc

         using (var outer = new ConfigurationContainer(operations.ToArray()).CreateScope(options.ArrangementWithMode(), logger, options.GetParameters())) {

            var process = outer.Resolve<Process>();

            if (process.Errors().Any()) {
               Environment.Exit(1);
            }

            var providers = new List<Autofac.Core.IModule> {
               new ConsoleProviderModule()
            };

            var output = process.GetOutputConnection();

            if (output == null || output.Provider == "internal" || output.Provider == "console") {
               logger.SuppressConsole();
               if (options.Format == "csv") {
                  output.Provider = "file";  // delimited file
                  output.Delimiter = ",";
                  output.Stream = true;
                  output.File = "dummy.csv";
                  providers.Add(new CsvHelperProviderModule(Console.OpenStandardOutput()));
               } else {
                  output.Provider = "json";
                  output.Stream = true;
                  output.Format = "json";
                  output.File = "dummy.json";
                  providers.Add(new JsonProviderModule(Console.OpenStandardOutput()));
               }
            } else {
               providers.Add(new CsvHelperProviderModule());
               providers.Add(new JsonProviderModule());
            }

            // PROVIDERS
            providers.Add(new AdoProviderModule());
            providers.Add(new BogusModule());
            providers.Add(new SqliteModule());
            providers.Add(new SqlServerModule());
            providers.Add(new PostgreSqlModule());
            // mysql
            providers.Add(new ElasticsearchModule());
            // solr
            // razor (templates)

            var modules = providers.Union(operations).ToArray();

            using (var inner = new Container(modules).CreateScope(process, logger)) {
               inner.Resolve<IProcessController>().Execute();
            }
         }
      }
      static void CommandLineError(IEnumerable<Error> errors) {
         Environment.Exit(1);
      }


   }
}
