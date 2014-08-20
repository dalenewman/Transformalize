using System;
using Transformalize.Libs.Lucene.Net.Analysis;
using Transformalize.Libs.Lucene.Net.Analysis.Standard;
using Version = Transformalize.Libs.Lucene.Net.Util.Version;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class LuceneAnalyzerFactory {
        public static Analyzer Create(string analyzer, string version) {
            switch (analyzer.ToLower()) {
                case "standard":
                    Version v;
                    if (version == Common.DefaultValue || !Enum.TryParse(version, true, out v)) {
                        v = Version.LUCENE_30;
                    }
                    return new StandardAnalyzer(v);
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