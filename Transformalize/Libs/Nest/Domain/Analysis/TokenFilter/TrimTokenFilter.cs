namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
	/// <summary>
	/// The trim token filter trims surrounding whitespaces around a token.
	/// </summary>
	public class TrimTokenFilter : TokenFilterBase
	{
		public TrimTokenFilter()
			: base("trim")
		{

		}

	}

}