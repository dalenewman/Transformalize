using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface ISegmentsResponse : IResponse
	{
		ShardsMetaData Shards { get; }
		Dictionary<string, IndexSegment> Indices { get; set; }
	}

	[JsonObject]
	public class SegmentsResponse : BaseResponse, ISegmentsResponse
	{

		[JsonProperty(PropertyName = "_shards")]
		public ShardsMetaData Shards { get; internal set; }

		[JsonProperty(PropertyName = "indices")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, IndexSegment> Indices { get; set; } 

		
	}
}