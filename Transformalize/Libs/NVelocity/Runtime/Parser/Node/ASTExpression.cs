using System;
using Transformalize.Libs.NVelocity.Context;

namespace Transformalize.Libs.NVelocity.Runtime.Parser.Node
{
    public class ASTExpression : SimpleNode
	{
		public ASTExpression(int id) : base(id)
		{
		}

		public ASTExpression(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			return GetChild(0).Evaluate(context);
		}

		public override Object Value(IInternalContextAdapter context)
		{
			return GetChild(0).Value(context);
		}
	}
}