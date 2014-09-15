using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<ValueCountAggregator>))]
	public interface IValueCountAggregator : IMetricAggregator
	{
	}
	
	public class ValueCountAggregator : MetricAggregator, IValueCountAggregator
	{
	}

	public class ValueCountAggregationDescriptor<T> : MetricAggregationBaseDescriptor<ValueCountAggregationDescriptor<T>, T>, IValueCountAggregator where T : class
	{
		
	}
}