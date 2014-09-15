using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface ICountResponse : IResponse
	{
		long Count { get; }
		ShardsMetaData Shards { get; }
	}

	[JsonObject]
	public class CountResponse : BaseResponse, ICountResponse
	{
		public CountResponse()
		{
			this.IsValid = true;
		}

		[JsonProperty(PropertyName = "count")]
		public long Count { get; internal set; }

		[JsonProperty(PropertyName = "_shards")]
		public ShardsMetaData Shards { get; internal set; }
	}
}