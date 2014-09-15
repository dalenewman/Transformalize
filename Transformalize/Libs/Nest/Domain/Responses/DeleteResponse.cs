using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IDeleteResponse : IResponse
	{
		string Id { get; }
		string Index { get; }
		string Type { get; }
		string Version { get; }
		bool Found { get; }
	}

	[JsonObject]
	public class DeleteResponse : BaseResponse, IDeleteResponse
	{
		[JsonProperty("_index")]
		public string Index { get; internal set; }
		[JsonProperty("_type")]
		public string Type { get; internal set; }
		[JsonProperty("_id")]
		public string Id { get; internal set; }
		[JsonProperty("_version")]
		public string Version { get; internal set; }
		[JsonProperty("found")]
		public bool Found { get; internal set; }

	}
}
