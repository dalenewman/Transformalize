#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Simple character tokenizer.
    /// </summary>
    internal class SimpleStringReader
    {
        private readonly string text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleStringReader" /> class.
        /// </summary>
        /// <param name="text">The text to be tokenized.</param>
        public SimpleStringReader(string text)
        {
            this.text = text;
            Position = 0;
        }

        internal int Position { get; set; }

        internal string Text
        {
            get { return text; }
        }

        internal int Peek()
        {
            if (Position < text.Length)
            {
                return text[Position];
            }

            return -1;
        }

        internal int Read()
        {
            if (Position < text.Length)
            {
                return text[Position++];
            }

            return -1;
        }

        internal string Substring(int p0, int p1)
        {
            return text.Substring(p0, p1 - p0);
        }
    }
}