using System.Collections.Generic;

namespace Transformalize.Libs.Nest.Domain.Aggregations
{
	public interface IBucketAggregation : IAggregation
	{
		IDictionary<string, IAggregation> Aggregations { get; }
	}
}