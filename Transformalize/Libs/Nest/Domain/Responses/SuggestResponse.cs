using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface ISuggestResponse : IResponse
	{
		ShardsMetaData Shards { get; }
		IDictionary<string, Suggest.Suggest[]> Suggestions { get; set; }
	}

	[JsonObject]
	public class SuggestResponse : BaseResponse, ISuggestResponse
	{
		public SuggestResponse()
		{
			this.IsValid = true;
			this.Suggestions = new Dictionary<string, Suggest.Suggest[]>();
		}
		
		public ShardsMetaData Shards { get; internal set; }

		public IDictionary<string, Suggest.Suggest[]> Suggestions { get; set;}
	}
}