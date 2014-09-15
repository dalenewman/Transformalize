namespace Transformalize.Libs.Nest.Domain.Analysis.Tokenizer
{
	/// <summary>
	/// A tokenizer of type whitespace that divides text at whitespace.
	/// </summary>
	public class WhitespaceTokenizer : TokenizerBase
    {
		public WhitespaceTokenizer()
        {
            Type = "whitespace";
        }
    }
}