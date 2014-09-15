using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<MaxAggregator>))]
	public interface IMaxAggregator : IMetricAggregator
	{
	}
	public class MaxAggregator : MetricAggregator, IMaxAggregator
	{
	}

	public class MaxAggregationDescriptor<T> : MetricAggregationBaseDescriptor<MaxAggregationDescriptor<T>, T>, IMaxAggregator where T : class
	{
		
	}
}
