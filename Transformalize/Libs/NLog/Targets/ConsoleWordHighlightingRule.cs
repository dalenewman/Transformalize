#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Transformalize.Libs.NLog.Config;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Highlighting rule for Win32 colorful console.
    /// </summary>
    [NLogConfigurationItem]
    public class ConsoleWordHighlightingRule
    {
        private Regex compiledRegex;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleWordHighlightingRule" /> class.
        /// </summary>
        public ConsoleWordHighlightingRule()
        {
            BackgroundColor = ConsoleOutputColor.NoChange;
            ForegroundColor = ConsoleOutputColor.NoChange;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleWordHighlightingRule" /> class.
        /// </summary>
        /// <param name="text">The text to be matched..</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public ConsoleWordHighlightingRule(string text, ConsoleOutputColor foregroundColor, ConsoleOutputColor backgroundColor)
        {
            Text = text;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }

        /// <summary>
        ///     Gets or sets the regular expression to be matched. You must specify either <c>text</c> or <c>regex</c>.
        /// </summary>
        /// <docgen category='Rule Matching Options' order='10' />
        public string Regex { get; set; }

        /// <summary>
        ///     Gets or sets the text to be matched. You must specify either <c>text</c> or <c>regex</c>.
        /// </summary>
        /// <docgen category='Rule Matching Options' order='10' />
        public string Text { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to match whole words only.
        /// </summary>
        /// <docgen category='Rule Matching Options' order='10' />
        [DefaultValue(false)]
        public bool WholeWords { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to ignore case when comparing texts.
        /// </summary>
        /// <docgen category='Rule Matching Options' order='10' />
        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        /// <summary>
        ///     Gets the compiled regular expression that matches either Text or Regex property.
        /// </summary>
        public Regex CompiledRegex
        {
            get
            {
                if (compiledRegex == null)
                {
                    var regexpression = Regex;

                    if (regexpression == null && Text != null)
                    {
                        regexpression = System.Text.RegularExpressions.Regex.Escape(Text);
                        if (WholeWords)
                        {
                            regexpression = "\b" + regexpression + "\b";
                        }
                    }

                    var regexOptions = RegexOptions.Compiled;
                    if (IgnoreCase)
                    {
                        regexOptions |= RegexOptions.IgnoreCase;
                    }

                    compiledRegex = new Regex(regexpression, regexOptions);
                }

                return compiledRegex;
            }
        }

        /// <summary>
        ///     Gets or sets the foreground color.
        /// </summary>
        /// <docgen category='Formatting Options' order='10' />
        [DefaultValue("NoChange")]
        public ConsoleOutputColor ForegroundColor { get; set; }

        /// <summary>
        ///     Gets or sets the background color.
        /// </summary>
        /// <docgen category='Formatting Options' order='10' />
        [DefaultValue("NoChange")]
        public ConsoleOutputColor BackgroundColor { get; set; }

        internal string MatchEvaluator(Match m)
        {
            var result = new StringBuilder();

            result.Append('\a');
            result.Append((char) ((int) ForegroundColor + 'A'));
            result.Append((char) ((int) BackgroundColor + 'A'));
            result.Append(m.Value);
            result.Append('\a');
            result.Append('X');

            return result.ToString();
        }

        internal string ReplaceWithEscapeSequences(string message)
        {
            return CompiledRegex.Replace(message, MatchEvaluator);
        }
    }
}

#endif