using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	[JsonObject]
	[JsonConverter(typeof(BulkOperationResponseItemConverter))]
	public class BulkCreateResponseItem : BulkOperationResponseItem
	{
		public override string Operation { get; internal set; }
		[JsonProperty("_index")]
		public override string Index { get; internal set; }
		[JsonProperty("_type")]
		public override string Type { get; internal set; }
		[JsonProperty("_id")]
		public override string Id { get; internal set; }
		[JsonProperty("_version")]
		public override string Version { get; internal set; }
		[JsonProperty("status")]
		public override int Status { get; internal set; }
		[JsonProperty("error")]
		public override string Error { get; internal set; }
	}
}