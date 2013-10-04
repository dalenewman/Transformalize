#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.Text;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers.Wrappers
{
    /// <summary>
    ///     Filters characters not allowed in the file names by replacing them with safe character.
    /// </summary>
    [LayoutRenderer("filesystem-normalize")]
    [AmbientProperty("FSNormalize")]
    [ThreadAgnostic]
    public sealed class FileSystemNormalizeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileSystemNormalizeLayoutRendererWrapper" /> class.
        /// </summary>
        public FileSystemNormalizeLayoutRendererWrapper()
        {
            FSNormalize = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to modify the output of this renderer so it can be used as a part of file path
        ///     (illegal characters are replaced with '_').
        /// </summary>
        /// <docgen category='Advanced Options' order='10' />
        [DefaultValue(true)]
        public bool FSNormalize { get; set; }

        /// <summary>
        ///     Post-processes the rendered message.
        /// </summary>
        /// <param name="text">The text to be post-processed.</param>
        /// <returns>Padded and trimmed string.</returns>
        protected override string Transform(string text)
        {
            if (FSNormalize)
            {
                var builder = new StringBuilder(text);
                for (var i = 0; i < builder.Length; i++)
                {
                    var c = builder[i];
                    if (!IsSafeCharacter(c))
                    {
                        builder[i] = '_';
                    }
                }

                return builder.ToString();
            }

            return text;
        }

        private static bool IsSafeCharacter(char c)
        {
            if (char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.' || c == ' ')
            {
                return true;
            }

            return false;
        }
    }
}