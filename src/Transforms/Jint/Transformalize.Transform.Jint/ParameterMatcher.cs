using Acornima;
using Acornima.Ast;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Transforms.Jint {

   public class ParameterMatcher : IParameterMatcher {

      public IEnumerable<string> Match(string script, IEnumerable<Field> available) {
         return new Parser().ParseScript(script)
           .DescendantNodesAndSelf()
           .Where(n => n.Type ==  NodeType.Identifier)
           .Select(n => n.As<Identifier>())
           .Select(i => i.Name)
           .Intersect(available.Select(f => f.Alias))
           .Distinct()
           .ToArray();
      }
   }
}
