using System.Collections.Generic;

namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class BlockStatement : Statement
    {
        public IEnumerable<Statement> Body;
    }
}