using System;
using System.Linq.Expressions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<NestedAggregator>))]
	public interface INestedAggregator : IBucketAggregator
	{
		[JsonProperty("path")] 
		PropertyPathMarker Path { get; set;}
	}

	public class NestedAggregator : BucketAggregator, INestedAggregator
	{
		public PropertyPathMarker Path { get; set; }
	}

	public class NestedAggregationDescriptor<T> : BucketAggregationBaseDescriptor<NestedAggregationDescriptor<T>, T>, INestedAggregator where T : class
	{
		PropertyPathMarker INestedAggregator.Path { get; set; }
		
		public NestedAggregationDescriptor<T> Path(string path)
		{
			((INestedAggregator)this).Path = path;
			return this;
		}

		public NestedAggregationDescriptor<T> Path(Expression<Func<T, object>> path)
		{
			((INestedAggregator)this).Path = path;
			return this;
		}
	}
}