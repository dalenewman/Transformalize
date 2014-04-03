//
// CSharpConvertBinder.cs
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
	class CSharpConvertBinder : ConvertBinder
	{
		readonly Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags;
		readonly Type context;

		public CSharpConvertBinder (Type type, Type context, Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags)
			: base (type, (flags & Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.ConvertExplicit) != 0)
		{
			this.flags = flags;
			this.context = context;
		}

        public override DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
		{
			var ctx = DynamicContext.Create ();
			var expr = ctx.CreateCompilerExpression (null, target);

			if (Explicit)
				expr = new Cast (new TypeExpression (ctx.ImportType (Type), Location.Null), expr, Location.Null);
			else
				expr = new ImplicitCast (expr, ctx.ImportType (Type), (flags & Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.ConvertArrayIndex) != 0);

			if ((flags & Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.CheckedContext) != 0)
				expr = new CheckedExpr (expr, Location.Null);

			var binder = new CSharpBinder (this, expr, errorSuggestion);
			binder.AddRestrictions (target);

			return binder.Bind (ctx, context);
		}
	}
}
