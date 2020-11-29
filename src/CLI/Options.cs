using System;
using CommandLine;

namespace Transformalize.Cli {

   public class Options {

      [Option('a', "arrangement", Required = true, HelpText = "An arrangement (aka configuration) file, or url. Note: you may add an optional query string.")]
      public string Arrangement { get; set; }

      [Option('m', "mode", Default = "default", Required = false, HelpText = "A system or user-defined mode (init, check, default, etc).")]
      public string Mode { get; set; }

      [Option('f', "format", Default = "csv", Required = false, HelpText = "Output format for console provider (csv, json).")]
      public string Format { get; set; }

      [Option('l',"log-level", Default = Contracts.LogLevel.Info, HelpText = "Sets the log level (Info, Debug, Warn, Error, None).")]
      public Contracts.LogLevel LogLevel { get; set; }

      public string ArrangementWithMode() {
         if (Mode == "default")
            return Arrangement;

         if (Arrangement.IndexOf("Mode=", StringComparison.OrdinalIgnoreCase) >= 0)
            return Arrangement;

         return Arrangement + (Arrangement.IndexOf('?') > 0 ? '&' : '?') + "Mode=" + Mode;
      }
   }
}