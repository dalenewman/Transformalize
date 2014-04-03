//
// CSharpGetMemberBinder.cs
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

namespace Transformalize.Libs.Mono.CSharp.RuntimeBinder
{
	class CSharpGetMemberBinder : GetMemberBinder
	{
		IList<CSharpArgumentInfo> argumentInfo;
		Type callingContext;
		
		public CSharpGetMemberBinder (string name, Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
			: base (name, false)
		{
			this.callingContext = callingContext;
			this.argumentInfo = argumentInfo.ToReadOnly ();
		}
		
		public override DynamicMetaObject FallbackGetMember (DynamicMetaObject target, DynamicMetaObject errorSuggestion)
		{
			var ctx = DynamicContext.Create ();

			var expr = ctx.CreateCompilerExpression (argumentInfo [0], target);
			expr = new MemberAccess (expr, Name);
			expr = new Cast (new TypeExpression (ctx.ImportType (ReturnType), Location.Null), expr, Location.Null);

			var binder = new CSharpBinder (this, expr, errorSuggestion);
			binder.AddRestrictions (target);

			return binder.Bind (ctx, callingContext);
		}
	}
}
