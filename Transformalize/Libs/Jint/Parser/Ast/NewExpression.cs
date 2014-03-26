using System.Collections.Generic;

namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class NewExpression : Expression
    {
        public Expression Callee;
        public IEnumerable<Expression> Arguments;
    }
}