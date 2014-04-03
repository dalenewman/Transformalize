//
// CSharpBinaryOperationBinder.cs
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
using System.Runtime.CompilerServices;

namespace Transformalize.Libs.Mono.CSharp.RuntimeBinder
{
	class CSharpBinaryOperationBinder : BinaryOperationBinder
	{
		IList<CSharpArgumentInfo> argumentInfo;
		readonly Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags;
		readonly Type context;
		
		public CSharpBinaryOperationBinder (ExpressionType operation, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo)
			: base (operation)
		{
			this.argumentInfo = new ReadOnlyCollectionBuilder<CSharpArgumentInfo> (argumentInfo);
			if (this.argumentInfo.Count != 2)
				throw new ArgumentException ("Binary operation requires 2 arguments");

			this.flags = flags;
			this.context = context;
		}

		Binary.Operator GetOperator (out bool isCompound)
		{
			isCompound = false;
			switch (Operation) {
			case ExpressionType.Add:
				return Binary.Operator.Addition;
			case ExpressionType.AddAssign:
				isCompound = true;
				return Binary.Operator.Addition;
			case ExpressionType.And:
				return (flags & Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.BinaryOperationLogical) != 0 ?
					Binary.Operator.LogicalAnd : Binary.Operator.BitwiseAnd;
			case ExpressionType.AndAssign:
				isCompound = true;
				return Binary.Operator.BitwiseAnd;
			case ExpressionType.Divide:
				return Binary.Operator.Division;
			case ExpressionType.DivideAssign:
				isCompound = true;
				return Binary.Operator.Division;
			case ExpressionType.Equal:
				return Binary.Operator.Equality;
			case ExpressionType.ExclusiveOr:
				return Binary.Operator.ExclusiveOr;
			case ExpressionType.ExclusiveOrAssign:
				isCompound = true;
				return Binary.Operator.ExclusiveOr;
			case ExpressionType.GreaterThan:
				return Binary.Operator.GreaterThan;
			case ExpressionType.GreaterThanOrEqual:
				return Binary.Operator.GreaterThanOrEqual;
			case ExpressionType.LeftShift:
				return Binary.Operator.LeftShift;
			case ExpressionType.LeftShiftAssign:
				isCompound = true;
				return Binary.Operator.LeftShift;
			case ExpressionType.LessThan:
				return Binary.Operator.LessThan;
			case ExpressionType.LessThanOrEqual:
				return Binary.Operator.LessThanOrEqual;
			case ExpressionType.Modulo:
				return Binary.Operator.Modulus;
			case ExpressionType.ModuloAssign:
				isCompound = true;
				return Binary.Operator.Modulus;
			case ExpressionType.Multiply:
				return Binary.Operator.Multiply;
			case ExpressionType.MultiplyAssign:
				isCompound = true;
				return Binary.Operator.Multiply;
			case ExpressionType.NotEqual:
				return Binary.Operator.Inequality;
			case ExpressionType.Or:
				return (flags & Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.BinaryOperationLogical) != 0 ?
					Binary.Operator.LogicalOr : Binary.Operator.BitwiseOr;
			case ExpressionType.OrAssign:
				isCompound = true;
				return Binary.Operator.BitwiseOr;
			case ExpressionType.OrElse:
				return Binary.Operator.LogicalOr;
			case ExpressionType.RightShift:
				return Binary.Operator.RightShift;
			case ExpressionType.RightShiftAssign:
				isCompound = true;
				return Binary.Operator.RightShift;
			case ExpressionType.Subtract:
				return Binary.Operator.Subtraction;
			case ExpressionType.SubtractAssign:
				isCompound = true;
				return Binary.Operator.Subtraction;
			default:
				throw new NotImplementedException (Operation.ToString ());
			}
		}
		
		public override DynamicMetaObject FallbackBinaryOperation (DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion)
		{
			var ctx = DynamicContext.Create ();
			var left = ctx.CreateCompilerExpression (argumentInfo [0], target);
			var right = ctx.CreateCompilerExpression (argumentInfo [1], arg);
			
			bool is_compound;
			var oper = GetOperator (out is_compound);
			Expression expr;

			if (is_compound) {
				var target_expr = new RuntimeValueExpression (target, ctx.ImportType (target.LimitType));
				expr = new CompoundAssign (oper, target_expr, right, left);
			} else {
				expr = new Binary (oper, left, right);
			}

			expr = new Cast (new TypeExpression (ctx.ImportType (ReturnType), Location.Null), expr, Location.Null);
			
			if ((flags & Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.CheckedContext) != 0)
				expr = new CheckedExpr (expr, Location.Null);

			var binder = new CSharpBinder (this, expr, errorSuggestion);
			binder.AddRestrictions (target);
			binder.AddRestrictions (arg);

			return binder.Bind (ctx, context);
		}
	}
}
