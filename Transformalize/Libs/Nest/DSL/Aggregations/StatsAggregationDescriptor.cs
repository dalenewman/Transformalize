using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<StatsAggregator>))]
	public interface IStatsAggregator : IMetricAggregator
	{
	}

	public class StatsAggregator : MetricAggregator, IStatsAggregator
	{
	}

	public class StatsAggregationDescriptor<T> : MetricAggregationBaseDescriptor<StatsAggregationDescriptor<T>, T>, IStatsAggregator where T : class
	{
		
	}
}
