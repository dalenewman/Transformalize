using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
    public abstract class TokenFilterBase : IAnalysisSetting
    {
        protected TokenFilterBase(string type)
        {
            this.Type = type;
        }
		
		[JsonProperty("version")]
	    public string Version { get; set; }

	    [JsonProperty("type")]
        public string Type { get; protected set; }
    }
}