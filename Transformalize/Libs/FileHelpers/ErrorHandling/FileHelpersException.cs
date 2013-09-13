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

namespace Transformalize.Libs.FileHelpers.ErrorHandling
{
    /// <summary>Base class for all the library Exceptions.</summary>
    public class FileHelpersException : Exception
    {
        /// <summary>Basic constructor of the exception.</summary>
        /// <param name="message">Message of the exception.</param>
        public FileHelpersException(string message) : base(message)
        {
        }

        /// <summary>Basic constructor of the exception.</summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerEx">The inner Exception.</param>
        public FileHelpersException(string message, Exception innerEx) : base(message, innerEx)
        {
        }
    }
}