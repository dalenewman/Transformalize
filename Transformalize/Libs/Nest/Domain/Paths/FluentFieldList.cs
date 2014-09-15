using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.Domain.Paths
{
	public class FluentFieldList<T> : List<PropertyPathMarker> where T : class
	{
		public FluentFieldList<T> Add(Expression<Func<T, object>> k)
		{
			base.Add(k);
			return this;
		}
		public FluentFieldList<T> Add(string k)
		{
			base.Add(k);
			return this;
		}
	}
}