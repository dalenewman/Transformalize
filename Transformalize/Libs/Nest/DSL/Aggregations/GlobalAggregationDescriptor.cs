using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Aggregations
{

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<GlobalAggregator>))]
	public interface IGlobalAggregator : IBucketAggregator
	{
		
	}

	public class GlobalAggregator : BucketAggregator, IGlobalAggregator
	{
		
	}

	public class GlobalAggregationDescriptor<T> : BucketAggregationBaseDescriptor<GlobalAggregationDescriptor<T>, T>, IGlobalAggregator
		where T : class
	{
	}
}