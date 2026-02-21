using Acornima;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Jint {

   public class ScriptTester {

      private readonly IContext _context;

      public ScriptTester(IContext context) {
         _context = context;
      }

      public bool Passes(string script) {
         try {
            var program = new Parser().ParseScript(script);
         } catch (ParseErrorException ex) {
            _context.Error(ex.Message);
            Utility.CodeToError(_context, script);
            return false;
         }
         return true;
      }
   }
}
