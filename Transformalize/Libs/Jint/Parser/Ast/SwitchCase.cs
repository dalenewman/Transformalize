using System.Collections.Generic;

namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class SwitchCase : SyntaxNode
    {
        public Expression Test;
        public IEnumerable<Statement> Consequent;
    }
}