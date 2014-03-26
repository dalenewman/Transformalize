using System.Collections.Generic;

namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class ArrayExpression : Expression
    {
        public IEnumerable<Expression> Elements;
    }
}