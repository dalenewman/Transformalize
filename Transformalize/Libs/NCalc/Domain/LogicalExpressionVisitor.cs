#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NCalc.Domain
{
    public abstract class LogicalExpressionVisitor
    {
        public abstract void Visit(LogicalExpression expression);
        public abstract void Visit(TernaryExpression expression);
        public abstract void Visit(BinaryExpression expression);
        public abstract void Visit(UnaryExpression expression);
        public abstract void Visit(ValueExpression expression);
        public abstract void Visit(Function function);
        public abstract void Visit(Identifier function);
    }
}