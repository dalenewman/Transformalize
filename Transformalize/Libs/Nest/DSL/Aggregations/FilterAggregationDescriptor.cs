using System;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.DSL.Filter;
using Transformalize.Libs.Nest.Resolvers.Converters.Aggregations;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(FilterAggregatorConverter))]
	public interface IFilterAggregator : IBucketAggregator
	{
		IFilterContainer Filter { get; set; }
	}

	public class FilterAggregator : BucketAggregator, IFilterAggregator
	{
		public IFilterContainer Filter { get; set; }
	}

	public class FilterAggregationDescriptor<T> : BucketAggregationBaseDescriptor<FilterAggregationDescriptor<T>, T> , IFilterAggregator 
		where T : class
	{
		IFilterContainer IFilterAggregator.Filter { get; set; }

		public FilterAggregationDescriptor<T> Filter(Func<FilterDescriptor<T>, FilterContainer> selector)
		{
			((IFilterAggregator)this).Filter = selector(new FilterDescriptor<T>());
			return this;
		}


	}
}