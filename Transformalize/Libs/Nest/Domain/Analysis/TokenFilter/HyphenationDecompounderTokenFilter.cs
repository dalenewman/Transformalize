namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
	public class HyphenationDecompounderTokenFilter : CompoundWordTokenFilter
	{
		public HyphenationDecompounderTokenFilter()
			: base("hyphenation_decompounder")
		{
		}
	}
}