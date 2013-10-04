#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NCalc.Domain
{
    public class Identifier : LogicalExpression
    {
        public Identifier(string name)
        {
            Name = name;
        }

        public string Name { get; set; }


        public override void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}