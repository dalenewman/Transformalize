using Autofac;
using Cfg.Net.Parsers.YamlDotNet;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Logging.MsLog;
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

      static int Main(string[] args) {

         // Shared options — Recursive = true so subcommands inherit them without re-declaration
         var arrangementOption = new Option<string>("--arrangement") {
            Description = "An arrangement (aka configuration) file, or url. Note: you may add an optional query string.",
            Required = true,
            Recursive = true
         };
         arrangementOption.Aliases.Add("-a");

         var formatOption = new Option<string>("--format") {
            Description = "Output format for console provider (csv, json).",
            DefaultValueFactory = _ => "csv",
            Recursive = true
         };
         formatOption.Aliases.Add("-f");

         var logLevelOption = new Option<LogLevel>("--log-level") {
            Description = "Sets the log level (Info, Debug, Warn, Error, None).",
            DefaultValueFactory = _ => LogLevel.Info,
            Recursive = true
         };
         logLevelOption.Aliases.Add("-l");

         var logFormatOption = new Option<string>("--log-format") {
            Description = "Sets the log format (text, json).",
            DefaultValueFactory = _ => "text",
            Recursive = true
         };

         // AllowMultipleArgumentsPerToken = true supports both syntaxes:
         //   -p key1=val1 -p key2=val2  (repeated flags, industry standard)
         //   -p key1=val1 key2=val2     (space-separated sequence)
         var parameterOption = new Option<string[]>("--parameter") {
            Description = "Add parameters that correspond to the arrangement's parameters and/or place holders.",
            AllowMultipleArgumentsPerToken = true,
            Recursive = true
         };
         parameterOption.Aliases.Add("-p");

         // Mode option is NOT recursive — init/schema verbs imply their mode, run gets its own instance
         var modeOption = new Option<string>("--mode") {
            Description = "A system or user-defined mode (init, schema, default, etc).",
            DefaultValueFactory = _ => "default"
         };
         modeOption.Aliases.Add("-m");

         // Helper to reduce duplication across actions
         RunOptions BuildOptions(ParseResult r, string mode) => new RunOptions {
            Arrangement = r.GetValue(arrangementOption),
            Mode = mode,
            Format = r.GetValue(formatOption),
            LogLevel = r.GetValue(logLevelOption),
            LogFormat = r.GetValue(logFormatOption) ?? "text",
            Parameters = r.GetValue(parameterOption) ?? Array.Empty<string>()
         };

         var rootCommand = new RootCommand("Run a Transformalize arrangement.");
         rootCommand.Options.Add(arrangementOption);
         rootCommand.Options.Add(modeOption);
         rootCommand.Options.Add(formatOption);
         rootCommand.Options.Add(logLevelOption);
         rootCommand.Options.Add(logFormatOption);
         rootCommand.Options.Add(parameterOption);

         // Root action — unchanged for backwards compatibility; -m init / -m schema still work here
         rootCommand.SetAction(parseResult => {
            Run(BuildOptions(parseResult, parseResult.GetValue(modeOption)));
            return 0;
         });

         // run verb — like root but -m is for custom user-defined modes only
         var runModeOption = new Option<string>("--mode") {
            Description = "A custom user-defined mode.",
            DefaultValueFactory = _ => "default"
         };
         runModeOption.Aliases.Add("-m");
         var runCommand = new Command("run", "Run your arrangement (same as the default).");
         runCommand.Options.Add(runModeOption);
         runCommand.SetAction(parseResult => {
            Run(BuildOptions(parseResult, parseResult.GetValue(runModeOption)));
            return 0;
         });

         // init verb — mode is implied, no -m needed or shown
         var initCommand = new Command("init", "Initialize your arrangement.");
         initCommand.SetAction(parseResult => {
            Run(BuildOptions(parseResult, "init"));
            return 0;
         });

         // schema verb — mode is implied, no -m needed or shown
         var schemaCommand = new Command("schema", "Detect and print the schema for your arrangement.");
         schemaCommand.SetAction(parseResult => {
            Run(BuildOptions(parseResult, "schema"));
            return 0;
         });

         rootCommand.Subcommands.Add(runCommand);
         rootCommand.Subcommands.Add(initCommand);
         rootCommand.Subcommands.Add(schemaCommand);

         return rootCommand.Parse(args).Invoke();
      }

      static void Run(RunOptions options) {

         IPipelineLogger logger = options.LogFormat == "json"
            ? new MsLogPipelineLogger(options.LogLevel, jsonFormat: true)
            : new ConsoleLogger(options.LogLevel);

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

            // if you are missing an output, or it's internal, or your output is console with csv or json format
            // wire up a csv or json provider that writes to the standard output
            if (output == null || output.Provider == "internal" || output.Provider == "console") {
               logger.SuppressConsole();
               if (output != null && output.Provider == "console" && output.Format == "text") {
                  // let the console writer handle it
               } else {
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
   }
}
