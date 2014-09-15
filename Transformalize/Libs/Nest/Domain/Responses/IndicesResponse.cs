using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Domain.Responses
{
    public interface IIndicesResponse : IResponse
    {
        bool Acknowledged { get; }
        ShardsMetaData ShardsHit { get; }
    }

    [JsonObject]
	public class IndicesResponse : BaseResponse, IIndicesResponse
    {
		[JsonProperty(PropertyName = "acknowledged")]
		public bool Acknowledged { get; private set; }

		[JsonProperty(PropertyName = "_shards")]
		public ShardsMetaData ShardsHit { get; private set; }
	}
}
