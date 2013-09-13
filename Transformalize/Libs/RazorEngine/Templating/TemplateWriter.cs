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
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines a template writer used for helper templates.
    /// </summary>
    public class TemplateWriter
    {
        #region Fields

        private readonly Action<TextWriter> writerDelegate;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialises a new instance of <see cref="TemplateWriter" />.
        /// </summary>
        /// <param name="writer">
        ///     The writer delegate used to write using the specified <see cref="TextWriter" />.
        /// </param>
        public TemplateWriter(Action<TextWriter> writer)
        {
            Contract.Requires(writer != null);

            writerDelegate = writer;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the write delegate and returns the result of this <see cref="TemplateWriter" />.
        /// </summary>
        /// <returns>The string result of the helper template.</returns>
        public override string ToString()
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writerDelegate(writer);
                return writer.ToString();
            }
        }

        /// <summary>
        ///     Writes the helper result of the specified text writer.
        /// </summary>
        /// <param name="writer">The text writer to write the helper result to.</param>
        public void WriteTo(TextWriter writer)
        {
            writerDelegate(writer);
        }

        #endregion
    }
}