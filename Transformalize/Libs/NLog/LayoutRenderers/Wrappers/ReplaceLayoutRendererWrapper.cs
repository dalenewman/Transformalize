#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text.RegularExpressions;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Replaces a string in the output of another layout with another string.
    /// </summary>
    [LayoutRenderer("replace")]
    [ThreadAgnostic]
    public sealed class ReplaceLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        private Regex regex;

        /// <summary>
        ///     Gets or sets the text to search for.
        /// </summary>
        /// <value>The text search for.</value>
        /// <docgen category='Search/Replace Options' order='10' />
        public string SearchFor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether regular expressions should be used.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if regular expressions should be used otherwise, <c>false</c>.
        /// </value>
        /// <docgen category='Search/Replace Options' order='10' />
        public bool Regex { get; set; }

        /// <summary>
        ///     Gets or sets the replacement string.
        /// </summary>
        /// <value>The replacement string.</value>
        /// <docgen category='Search/Replace Options' order='10' />
        public string ReplaceWith { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to ignore case.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if case should be ignored when searching; otherwise, <c>false</c>.
        /// </value>
        /// <docgen category='Search/Replace Options' order='10' />
        public bool IgnoreCase { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to search for whole words.
        /// </summary>
        /// <value>
        ///     A value of <c>true</c> if whole words should be searched for; otherwise, <c>false</c>.
        /// </value>
        /// <docgen category='Search/Replace Options' order='10' />
        public bool WholeWords { get; set; }

        /// <summary>
        ///     Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();
            var regexString = SearchFor;

            if (!Regex)
            {
                regexString = System.Text.RegularExpressions.Regex.Escape(regexString);
            }

#if SILVERLIGHT
            RegexOptions regexOptions = RegexOptions.None;
#else
            var regexOptions = RegexOptions.Compiled;
#endif
            if (IgnoreCase)
            {
                regexOptions |= RegexOptions.IgnoreCase;
            }

            if (WholeWords)
            {
                regexString = "\\b" + regexString + "\\b";
            }

            regex = new Regex(regexString, regexOptions);
        }

        /// <summary>
        ///     Post-processes the rendered message.
        /// </summary>
        /// <param name="text">The text to be post-processed.</param>
        /// <returns>Post-processed text.</returns>
        protected override string Transform(string text)
        {
            return regex.Replace(text, ReplaceWith);
        }
    }
}