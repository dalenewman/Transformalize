#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.IO;
using System.Text;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Renders contents of the specified file.
    /// </summary>
    [LayoutRenderer("file-contents")]
    public class FileContentsLayoutRenderer : LayoutRenderer
    {
        private string currentFileContents;
        private string lastFileName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileContentsLayoutRenderer" /> class.
        /// </summary>
        public FileContentsLayoutRenderer()
        {
#if SILVERLIGHT
            this.Encoding = Encoding.UTF8;
#else
            Encoding = Encoding.Default;
#endif
            lastFileName = string.Empty;
        }

        /// <summary>
        ///     Gets or sets the name of the file.
        /// </summary>
        /// <docgen category='File Options' order='10' />
        [DefaultParameter]
        public Layout FileName { get; set; }

        /// <summary>
        ///     Gets or sets the encoding used in the file.
        /// </summary>
        /// <value>The encoding.</value>
        /// <docgen category='File Options' order='10' />
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Renders the contents of the specified file and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            lock (this)
            {
                var fileName = FileName.Render(logEvent);

                if (fileName != lastFileName)
                {
                    currentFileContents = ReadFileContents(fileName);
                    lastFileName = fileName;
                }
            }

            builder.Append(currentFileContents);
        }

        private string ReadFileContents(string fileName)
        {
            try
            {
                using (var reader = new StreamReader(fileName, Encoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Cannot read file contents: {0} {1}", fileName, exception);
                return string.Empty;
            }
        }
    }
}