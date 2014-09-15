using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Analysis.CharFilter;
using Transformalize.Libs.Nest.Domain.Analysis.TokenFilter;
using Transformalize.Libs.Nest.Domain.Analysis.Tokenizer;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Analysis.Analyzers
{
	[JsonConverter(typeof(AnalysisSettingsConverter))]
	public class AnalysisSettings
    {
        public AnalysisSettings()
        {
			this.Analyzers = new Dictionary<string, AnalyzerBase>();
            this.TokenFilters = new Dictionary<string, TokenFilterBase>();
			this.Tokenizers = new Dictionary<string, TokenizerBase>();
			this.CharFilters = new Dictionary<string, CharFilterBase>();
        }

		[JsonConverter(typeof(AnalyzerCollectionConverter))]
		public IDictionary<string, AnalyzerBase> Analyzers { get; set; }

		[JsonConverter(typeof(TokenFilterCollectionConverter))]
		public IDictionary<string, TokenFilterBase> TokenFilters { get; set; }
	
		[JsonConverter(typeof(TokenizerCollectionConverter))]
		public IDictionary<string, TokenizerBase> Tokenizers { get; set; }
		
		[JsonConverter(typeof(CharFilterCollectionConverter))]
		public IDictionary<string, CharFilterBase> CharFilters { get; set; }
    }
}