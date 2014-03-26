using System.Collections.Generic;

namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class SequenceExpression : Expression
    {
        public IList<Expression> Expressions;
    }
}