using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Responses
{
    public interface IIndicesOperationResponse : IResponse
    {
        bool Acknowledged { get; }
    }

    [JsonObject]
	public class IndicesOperationResponse : BaseResponse, IIndicesOperationResponse
    {
		public IndicesOperationResponse()
		{
			this.IsValid = true;
		}

		[JsonProperty(PropertyName = "acknowledged")]
		public bool Acknowledged { get; internal set; }
	}
}