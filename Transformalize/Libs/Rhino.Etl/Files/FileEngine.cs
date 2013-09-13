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
using System.Collections;
using Transformalize.Libs.FileHelpers.Engines;
using Transformalize.Libs.FileHelpers.Enums;

namespace Rhino.Etl.Core.Files
{
    /// <summary>
    ///     Adapter class to facilitate the nicer syntax
    /// </summary>
    public class FileEngine : IDisposable, IEnumerable
    {
        private readonly FileHelperAsyncEngine engine;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileEngine" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public FileEngine(FileHelperAsyncEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return engine.ErrorManager.HasErrors; }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IDisposable d = engine;
            d.Dispose();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            IEnumerable e = engine;
            return e.GetEnumerator();
        }

        /// <summary>
        ///     Writes the specified object ot the file
        /// </summary>
        /// <param name="t">The t.</param>
        public void Write(object t)
        {
            engine.WriteNext(t);
        }

        /// <summary>
        ///     Set the behavior on error
        /// </summary>
        /// <param name="errorMode">The error mode.</param>
        public FileEngine OnError(ErrorMode errorMode)
        {
            engine.ErrorManager.ErrorMode = errorMode;
            return this;
        }

        /// <summary>
        ///     Outputs the errors to the specified file
        /// </summary>
        /// <param name="file">The file.</param>
        public void OutputErrors(string file)
        {
            engine.ErrorManager.SaveErrors(file);
        }
    }
}