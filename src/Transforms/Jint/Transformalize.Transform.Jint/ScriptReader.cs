using Cfg.Net.Contracts;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Jint {
   public class ScriptReader {

      private readonly IContext _context;
      private readonly IReader _reader;

      public ScriptReader(IContext context, IReader reader) {
         _context = context;
         _reader = reader;
      }

      /// <summary>
      /// Read the script.  The script could be from the content attribute, 
      /// from a file referenced in the file attribute, or a combination.
      /// </summary>
      /// <param name="context"></param>
      /// <param name="reader"></param>
      /// <param name="script"></param>
      /// <returns></returns>
      public string Read(Script script) {
         var content = string.Empty;

         if (script.Content != string.Empty)
            content += script.Content + "\r\n";

         if (script.File != string.Empty) {
            var l = new Cfg.Net.Loggers.MemoryLogger();
            var response = _reader.Read(script.File, new Dictionary<string, string>(), l);
            if (l.Errors().Any()) {
               foreach (var error in l.Errors()) {
                  _context.Error(error);
               }
               _context.Error($"Could not load {script.File}.");
            } else {
               content += response + "\r\n";
            }
         }

         return content;
      }
   }
}
