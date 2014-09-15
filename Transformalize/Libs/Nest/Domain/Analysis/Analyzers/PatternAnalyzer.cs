using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Analysis.Analyzers
{
	/// <summary>
	/// An analyzer of type pattern that can flexibly separate text into terms via a regular expression. 
	/// </summary>
	public class PatternAnalyzer : AnalyzerBase
    {
		public PatternAnalyzer()
        {
            Type = "pattern";
        }

		[JsonProperty("lowercase")]
		public bool? Lowercase { get; set; }

		[JsonProperty("pattern")]
		public string Pattern { get; set; }

		[JsonProperty("flags")]
		public string Flags { get; set; }
    }
}