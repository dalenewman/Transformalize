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

namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
    /// <summary>
    ///     Interface implemented by all factories capable of creating file appenders.
    /// </summary>
    internal interface IFileAppenderFactory
    {
        /// <summary>
        ///     Opens the appender for given file name and parameters.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="parameters">Creation parameters.</param>
        /// <returns>
        ///     Instance of <see cref="BaseFileAppender" /> which can be used to write to the file.
        /// </returns>
        BaseFileAppender Open(string fileName, ICreateFileParameters parameters);
    }
}