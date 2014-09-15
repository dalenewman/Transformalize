using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IEmptyResponse : IResponse
	{
	}

	[JsonObject]
	public class EmptyResponse : BaseResponse, IEmptyResponse
	{
		public EmptyResponse()
		{
			this.IsValid = true;
		}
	}
}