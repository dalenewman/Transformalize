using System;
using System.Collections.Generic;
using CommandLine;

namespace Transformalize.Cli {

   [Verb("run", isDefault:true, HelpText = "Run your arrangement (the default).")]
   public class RunOptions {

      // would like to make this an a value parameter option but it gets confused with the verb
      [Option('a', "arrangement", Required = true, HelpText = "An arrangement (aka configuration) file, or url. Note: you may add an optional query string.")]
      public string Arrangement { get; set; }

      [Option('m', "mode", Default = "default", Required = false, HelpText = "A system or user-defined mode (init, check, default, etc).")]
      public string Mode { get; set; }

      [Option('f', "format", Default = "csv", Required = false, HelpText = "Output format for console provider (csv, json).")]
      public string Format { get; set; }

      [Option('l', "log-level", Default = Contracts.LogLevel.Info, HelpText = "Sets the log level (Info, Debug, Warn, Error, None).")]
      public Contracts.LogLevel LogLevel { get; set; }

      [Option('p', "parameter", HelpText = "Add parameters that correspond to the arrangement's parameters and/or place holders.")]
      public IEnumerable<string> Parameters { get; set; }

      public Dictionary<string, string> GetParameters() {
         var parameters = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

         if (Parameters != null) {
            foreach (var parameter in Parameters) {
               if (parameter.Contains('=')) {
                  var split = parameter.Split('=');
                  parameters[split[0]] = split[1];
               }
            }
         }
         return parameters;
      }

      public string ArrangementWithMode() {
         if (Mode == "default")
            return Arrangement;

         if (Arrangement.IndexOf("Mode=", StringComparison.OrdinalIgnoreCase) >= 0)
            return Arrangement;

         return Arrangement + (Arrangement.IndexOf('?') > 0 ? '&' : '?') + "Mode=" + Mode;
      }
   }
}