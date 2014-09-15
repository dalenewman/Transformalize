using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<ExtendedStatsAggregator>))]
	public interface IExtendedStatsAggregator : IMetricAggregator
	{
	}

	public class ExtendedStatsAggregator : MetricAggregator, IExtendedStatsAggregator { }

	public class ExtendedStatsAggregationDescriptor<T> : MetricAggregationBaseDescriptor<ExtendedStatsAggregationDescriptor<T>, T>, IExtendedStatsAggregator where T : class
	{
		
	}
}