using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<SumAggregator>))]
	public interface ISumAggregator : IMetricAggregator
	{
	}

	public class SumAggregator : MetricAggregator, ISumAggregator
	{
	}

	public class SumAggregationDescriptor<T> : MetricAggregationBaseDescriptor<SumAggregationDescriptor<T>, T>, ISumAggregator where T : class
	{
		
	}
}
