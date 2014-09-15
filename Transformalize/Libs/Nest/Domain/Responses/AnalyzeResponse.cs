using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Domain.Responses
{
    public interface IAnalyzeResponse : IResponse
    {
        IEnumerable<AnalyzeToken> Tokens { get; }
    }

    [JsonObject]
	public class AnalyzeResponse : BaseResponse, IAnalyzeResponse
    {
		public AnalyzeResponse()
		{
			this.IsValid = true;
		}
		[JsonProperty(PropertyName = "tokens")]
		public IEnumerable<AnalyzeToken> Tokens { get; internal set; }
	}
}