using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IShardsOperationResponse : IResponse
	{
		ShardsMetaData Shards { get; }
	}

	public class ShardsOperationResponse : BaseResponse, IShardsOperationResponse
	{
		[JsonProperty("_shards")]
		public ShardsMetaData Shards { get; internal set; }
	}
}