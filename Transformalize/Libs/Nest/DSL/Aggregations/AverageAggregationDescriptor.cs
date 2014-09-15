using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<AverageAggregator>))]
	public interface IAverageAggregator : IMetricAggregator
	{
	}

	public class AverageAggregator : MetricAggregator, IAverageAggregator
	{
		
	}

	public class AverageAggregationDescriptor<T> : MetricAggregationBaseDescriptor<AverageAggregationDescriptor<T>, T>, IAverageAggregator where T : class
	{
		
	}
}
