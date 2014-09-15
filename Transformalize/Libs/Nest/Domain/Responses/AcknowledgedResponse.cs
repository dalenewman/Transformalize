using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IAcknowledgedResponse : IResponse
	{
		bool Acknowledged { get; }
	}

	[JsonObject]
	public class AcknowledgedResponse : BaseResponse, IAcknowledgedResponse
	{
		[JsonProperty(PropertyName = "acknowledged")]
		public bool Acknowledged { get; internal set; }
	}
}