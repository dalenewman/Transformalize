namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
	/// <summary>
	/// A token filter of type standard that normalizes tokens extracted with the Standard Tokenizer.
	/// </summary>
	public class StandardTokenFilter : TokenFilterBase
	{
		public StandardTokenFilter()
			: base("standard")
		{

		}

	}

}