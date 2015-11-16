using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneAnalyzerFactory {
        public static Analyzer Create(string analyzer, string version) {
            switch (analyzer.ToLower()) {
                case "standard":
                    return new StandardAnalyzer(LuceneConnection.GetLuceneVersion(version));
                case "simple":
                    return new SimpleAnalyzer();
                case "whitespace":
                    return new WhitespaceAnalyzer();
                default:
                    return new KeywordAnalyzer();
            }

        }
    }
}