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
using Transformalize.Libs.FileHelpers.Engines;

namespace Rhino.Etl.Core.Files
{
    /// <summary>
    ///     Gives a fluent interface syntax on top of the <see cref="FileHelperEngine" />
    /// </summary>
    public class FluentFile
    {
        private readonly FileHelperAsyncEngine engine;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentFile" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public FluentFile(Type type)
        {
            engine = new FileHelperAsyncEngine(type);
        }

        /// <summary>
        ///     Gets or sets the footer text.
        /// </summary>
        /// <value>The footer text.</value>
        public string FooterText
        {
            get { return engine.FooterText; }
            set { engine.FooterText = value; }
        }

        /// <summary>
        ///     Gets or sets the header text.
        /// </summary>
        /// <value>The header text.</value>
        public string HeaderText
        {
            get { return engine.HeaderText; }
            set { engine.HeaderText = value; }
        }

        /// <summary>
        ///     Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding
        {
            get { return engine.Encoding; }
            set { engine.Encoding = value; }
        }

        /// <summary>
        ///     Get a new fluent file instance for
        ///     <typeparam name="T"></typeparam>
        /// </summary>
        public static FluentFile For<T>()
        {
            return new FluentFile(typeof (T));
        }

        /// <summary>
        ///     Specify which file to start reading from
        /// </summary>
        /// <param name="filename">The filename.</param>
        public FileEngine From(string filename)
        {
            filename = NormalizeFilename(filename);
            engine.BeginReadFile(filename);
            return new FileEngine(engine);
        }

        /// <summary>
        ///     Specify which file to start writing to
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>
        ///     This will overwrite the file, use <see cref="AppendTo" /> if you want
        ///     to append.
        /// </remarks>
        public FileEngine To(string filename)
        {
            filename = NormalizeFilename(filename);
            engine.BeginWriteFile(filename);
            return new FileEngine(engine);
        }

        /// <summary>
        ///     Specify which file to start appending to
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public FileEngine AppendTo(string filename)
        {
            engine.BeginAppendToFile(filename);
            return new FileEngine(engine);
        }

        private static string NormalizeFilename(string filename)
        {
            if (filename.StartsWith("~") == false)
                return filename;
            //note that this ignores rooted paths
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
        }

        ///// <summary>
        ///// Gets or sets the options.
        ///// </summary>
        ///// <value>The options.</value>
        //public RecordOptions Options
        //{
        //    get { return engine.RecordType.; }
        //    set { engine.Options = value; }
        //}
    }
}