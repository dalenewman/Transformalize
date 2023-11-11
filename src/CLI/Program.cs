using Autofac;
using Cfg.Net.Parsers.YamlDotNet;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.Console.Autofac;
using Transformalize.Providers.CsvHelper.Autofac;
using Transformalize.Providers.Elasticsearch.Autofac;
using Transformalize.Providers.Json.Autofac;
using Transformalize.Providers.Mail.Autofac;
using Transformalize.Providers.MySql.Autofac;
using Transformalize.Providers.PostgreSql.Autofac;
using Transformalize.Providers.Razor.Autofac;
using Transformalize.Providers.Solr.Autofac;
using Transformalize.Providers.Sqlite.Autofac;
using Transformalize.Providers.SqlServer.Autofac;
using Transformalize.Transform.GoogleMaps;
using Transformalize.Transforms.Compression;
using Transformalize.Transforms.Fluid.Autofac;
using Transformalize.Transforms.Geography;
using Transformalize.Transforms.Globalization;
using Transformalize.Transforms.Humanizer.Autofac;
using Transformalize.Transforms.Jint.Autofac;
using Transformalize.Transforms.Json.Autofac;
using Transformalize.Transforms.LambdaParser.Autofac;
using Transformalize.Transforms.Razor.Autofac;
using Transformalize.Transforms.Xml;

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
            new JintTransformModule(),
            new RazorTransformModule(),
            new FluidTransformModule(),
            new HumanizeModule(),
            new LambdaParserModule(),
            new JsonTransformModule()
         };

         // adding transforms that aren't in modules
         var container = new ConfigurationContainer(operations.ToArray());
         container.AddTransform((c) => new SlugifyTransform(c), new SlugifyTransform().GetSignatures());
         container.AddTransform((c) => new DistanceTransform(c), new DistanceTransform().GetSignatures());
         container.AddTransform((c) => new GeohashEncodeTransform(c), new GeohashEncodeTransform().GetSignatures());
         container.AddTransform((c) => new GeohashNeighborTransform(c), new GeohashNeighborTransform().GetSignatures());
         container.AddTransform((c) => new CompressTransform(c), new CompressTransform().GetSignatures());
         container.AddTransform((c) => new DecompressTransform(c), new DecompressTransform().GetSignatures());
         container.AddTransform((c) => new FromXmlTransform(c), new FromXmlTransform().GetSignatures());
         container.AddTransform((c) => new XPathTransform(c), new XPathTransform().GetSignatures());
         container.AddTransform((c) => new GeocodeTransform(c), new GeocodeTransform().GetSignatures());
         container.AddTransform((c) => new PlaceTransform(c), new PlaceTransform().GetSignatures());

         var placeHolderStyle = "@[]"; // the legacy style
         if (options.Arrangement.EndsWith("yaml", StringComparison.OrdinalIgnoreCase) || options.Arrangement.EndsWith("yml", StringComparison.OrdinalIgnoreCase)) {
            container.AddDependency(new YamlDotNetParser());
            placeHolderStyle = "${}";
         }

         using (var outer = container.CreateScope(options.ArrangementWithMode(), logger, options.GetParameters(), placeHolderStyle)) {

            var process = outer.Resolve<Process>();

            if (options.Mode != "default" && options.Mode != process.Mode) {
               process.Mode = options.Mode;
               process.Load();
            }

            if (process.Errors().Any()) {
               var context = new PipelineContext(logger, process);
               context.Error("The configuration has errors.");
               foreach (var error in process.Errors()) {
                  context.Error(error);
               }
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
                  output.Synchronous = true; // got odd results when using Async methods
                  output.File = "dummy.csv";
                  providers.Add(new CsvHelperProviderModule(new System.IO.StreamWriter(Console.OpenStandardOutput())));
               } else {
                  output.Provider = "json";
                  output.Stream = true;
                  output.Format = "json";
                  output.File = "dummy.json";
                  providers.Add(new JsonProviderModule(new System.IO.StreamWriter(Console.OpenStandardOutput())));
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
            providers.Add(new MySqlModule());
            providers.Add(new ElasticsearchModule());
            providers.Add(new RazorProviderModule());
            providers.Add(new MailModule());
            providers.Add(new SolrModule());

            var modules = providers.Union(operations).ToArray();

            if (options.Mode.ToLower() == "schema") {
               using (var inner = new Container(modules).CreateScope(process, logger)) {
                  process = new SchemaService(inner).GetProcess(process);
                  process.Connections.Clear();
                  Console.WriteLine(process.Serialize());
                  Environment.Exit(0);
               }
            } else if (process.Entities.Count == 1 && !process.Entities[0].Fields.Any(f => f.Input)) {
               using (var inner = new Container(modules).CreateScope(process, logger)) {
                  if (new SchemaService(inner).Help(process)) {
                     process.Load();
                  } else {
                     Console.Error.WriteLine($"Unable to detect fields in {process.Entities[0].Name}.");
                     Environment.Exit(1);
                  }
               }
            }

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
