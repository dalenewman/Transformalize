namespace Transformalize.Libs.Nest.Domain.Aggregations
{
	public class KeyItem : BucketAggregationBase, IBucketItem
	{
		public string Key { get; set; }
		public long DocCount { get; set; }
	}
}