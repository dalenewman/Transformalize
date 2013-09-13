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


#if !SILVERLIGHT2 && !SILVERLIGHT3 && !WINDOWS_PHONE

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Line ending mode.
    /// </summary>
    public enum LineEndingMode
    {
        /// <summary>
        ///     Insert platform-dependent end-of-line sequence after each line.
        /// </summary>
        Default,

        /// <summary>
        ///     Insert CR LF sequence (ASCII 13, ASCII 10) after each line.
        /// </summary>
        CRLF,

        /// <summary>
        ///     Insert CR character (ASCII 13) after each line.
        /// </summary>
        CR,

        /// <summary>
        ///     Insert LF character (ASCII 10) after each line.
        /// </summary>
        LF,

        /// <summary>
        ///     Don't insert any line ending.
        /// </summary>
        None,
    }
}

#endif