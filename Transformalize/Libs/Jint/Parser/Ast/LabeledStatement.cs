namespace Transformalize.Libs.Jint.Parser.Ast
{
    public class LabelledStatement : Statement
    {
        public Identifier Label;
        public Statement Body;
    }
}