//
// CSharpUnaryOperationBinder.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2009 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace Transformalize.Libs.Mono.CSharp.RuntimeBinder
{
	class CSharpUnaryOperationBinder : UnaryOperationBinder
	{
		IList<CSharpArgumentInfo> argumentInfo;
		readonly Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags;
		readonly Type context;
		
		public CSharpUnaryOperationBinder (ExpressionType operation, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo)
			: base (operation)
		{
			this.argumentInfo = argumentInfo.ToReadOnly ();
			if (this.argumentInfo.Count != 1)
				throw new ArgumentException ("Unary operation requires 1 argument");

			this.flags = flags;
			this.context = context;
		}
	

		Unary.Operator GetOperator ()
		{
			switch (Operation) {
			case ExpressionType.Negate:
				return Unary.Operator.UnaryNegation;
			case ExpressionType.Not:
				return Unary.Operator.LogicalNot;
			case ExpressionType.OnesComplement:
				return Unary.Operator.OnesComplement;
			case ExpressionType.UnaryPlus:
				return Unary.Operator.UnaryPlus;
			default:
				throw new NotImplementedException (Operation.ToString ());
			}
		}
		
		public override DynamicMetaObject FallbackUnaryOperation (DynamicMetaObject target, DynamicMetaObject errorSuggestion)
		{
			var ctx = DynamicContext.Create ();
			var expr = ctx.CreateCompilerExpression (argumentInfo [0], target);

			if (Operation == ExpressionType.IsTrue) {
				expr = new BooleanExpression (expr);
			} else if (Operation == ExpressionType.IsFalse) {
				expr = new BooleanExpressionFalse (expr);
			} else {
				if (Operation == ExpressionType.Increment)
					expr = new UnaryMutator (UnaryMutator.Mode.PreIncrement, expr, Location.Null);
				else if (Operation == ExpressionType.Decrement)
					expr = new UnaryMutator (UnaryMutator.Mode.PreDecrement, expr, Location.Null);
				else
					expr = new Unary (GetOperator (), expr, Location.Null);

				expr = new Cast (new TypeExpression (ctx.ImportType (ReturnType), Location.Null), expr, Location.Null);

				if ((flags & Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.CheckedContext) != 0)
					expr = new CheckedExpr (expr, Location.Null);
			}

			var binder = new CSharpBinder (this, expr, errorSuggestion);
			binder.AddRestrictions (target);

			return binder.Bind (ctx, context);
		}
	}
}
