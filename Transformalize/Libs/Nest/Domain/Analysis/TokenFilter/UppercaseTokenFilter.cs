namespace Transformalize.Libs.Nest.Domain.Analysis.TokenFilter
{
    /// <summary>
    /// A token filter of type uppercase that normalizes token text to upper case.
    /// </summary>
    public class UppercaseTokenFilter : TokenFilterBase
    {
        public UppercaseTokenFilter()
            : base("uppercase")
        {

        }

    }
}