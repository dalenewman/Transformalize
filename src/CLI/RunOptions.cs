using System;
using System.Collections.Generic;

namespace Transformalize.Cli {

   public class RunOptions {

      public string Arrangement { get; set; }
      public string Mode { get; set; } = "default";
      public string Format { get; set; } = "csv";
      public Contracts.LogLevel LogLevel { get; set; } = Contracts.LogLevel.Info;
      public string[] Parameters { get; set; } = Array.Empty<string>();

      public Dictionary<string, string> GetParameters() {
         var parameters = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
         foreach (var parameter in Parameters) {
            var idx = parameter.IndexOf('=');
            if (idx > 0) {
               parameters[parameter[..idx]] = parameter[(idx + 1)..];
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
