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


#if !NET_CF

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Format of the ${stacktrace} layout renderer output.
    /// </summary>
    public enum StackTraceFormat
    {
        /// <summary>
        ///     Raw format (multiline - as returned by StackFrame.ToString() method).
        /// </summary>
        Raw,

        /// <summary>
        ///     Flat format (class and method names displayed in a single line).
        /// </summary>
        Flat,

        /// <summary>
        ///     Detailed flat format (method signatures displayed in a single line).
        /// </summary>
        DetailedFlat,
    }
}

#endif