#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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