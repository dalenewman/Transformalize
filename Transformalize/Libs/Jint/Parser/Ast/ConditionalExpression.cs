namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class ConditionalExpression : Expression
    {
        public Expression Test;
        public Expression Consequent;
        public Expression Alternate;
    }
}