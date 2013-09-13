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