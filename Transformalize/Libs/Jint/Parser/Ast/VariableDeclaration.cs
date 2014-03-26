using System.Collections.Generic;

namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class VariableDeclaration : Statement
    {
        public IEnumerable<VariableDeclarator> Declarations;
        public string Kind;
    }
}