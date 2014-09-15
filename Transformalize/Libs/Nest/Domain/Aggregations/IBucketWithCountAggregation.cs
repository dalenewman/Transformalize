namespace Transformalize.Libs.Nest.Domain.Aggregations
{
	public interface IBucketWithCountAggregation : IBucketAggregation
	{
		long DocCount { get; }
	}
}