using System.Collections.Generic;

namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class SwitchStatement : Statement
    {
        public Expression Discriminant;
        public IEnumerable<SwitchCase> Cases;
    }
}