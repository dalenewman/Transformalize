namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
    /// <summary>
	/// A token filter of type porterStem that transforms the token stream as per the Porter stemming algorithm.
    /// </summary>
    public class PorterStemTokenFilter : TokenFilterBase
    {
		public PorterStemTokenFilter()
            : base("porterStem")
        {

        }

    }
}