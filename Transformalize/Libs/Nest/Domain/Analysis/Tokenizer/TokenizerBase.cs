using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.Tokenizer
{
	public abstract class TokenizerBase : IAnalysisSetting
	{
		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }
		
		[JsonProperty(PropertyName = "type")]
		public string Type { get; protected set; }
	}
}
