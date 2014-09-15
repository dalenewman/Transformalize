using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.CharFilter
{
    public abstract class CharFilterBase : IAnalysisSetting
    {
        protected CharFilterBase(string type)
        {
            this.Type = type;
        }
		
		[JsonProperty("version")]
	    public string Version { get; set; }

	    [JsonProperty("type")]
        public string Type { get; protected set; }
    }
}