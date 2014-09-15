using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<MinAggregator>))]
	public interface IMinAggregator : IMetricAggregator
	{
	}

	public class MinAggregator : MetricAggregator, IMinAggregator
	{
	}

	public class MinAggregationDescriptor<T> : MetricAggregationBaseDescriptor<MinAggregationDescriptor<T>, T>, IMinAggregator where T : class
	{
		
	}
}
