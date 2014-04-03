//
// CSharpIsEventBinder.cs
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
using System.Dynamic;

namespace Transformalize.Libs.Mono.CSharp.RuntimeBinder
{
	class CSharpIsEventBinder : DynamicMetaObjectBinder
	{
		Type callingContext;
		string name;
		
		public CSharpIsEventBinder (string name, Type callingContext)
		{
			this.name = name;
			this.callingContext = callingContext;
		}
		
		public override DynamicMetaObject Bind (DynamicMetaObject target, DynamicMetaObject[] args)
		{
			var ctx = DynamicContext.Create ();
			var context_type = ctx.ImportType (callingContext);
			var queried_type = ctx.ImportType (target.LimitType);
			var rc = new ResolveContext (new RuntimeBinderContext (ctx, context_type), 0);

			var expr = Expression.MemberLookup (rc, false, queried_type,
				name, 0, Expression.MemberLookupRestrictions.ExactArity, Location.Null);

			var binder = new CSharpBinder (
				this, new BoolConstant (ctx.CompilerContext.BuiltinTypes, expr is EventExpr, Location.Null), null);

			binder.AddRestrictions (target);
			return binder.Bind (ctx, callingContext);
		}

		public override Type ReturnType {
			get {
				return typeof (bool);
			}
		}
	}
}
