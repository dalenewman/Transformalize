using System;

namespace Transformalize.Libs.NVelocity.Runtime.Parser.Node
{
    public class ASTParameters : SimpleNode
	{
		public ASTParameters(int id) : base(id)
		{
		}

		public ASTParameters(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}
	}
}