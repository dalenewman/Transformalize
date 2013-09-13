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

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Value indicating how stack trace should be captured when processing the log event.
    /// </summary>
    public enum StackTraceUsage
    {
        /// <summary>
        ///     Stack trace should not be captured.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Stack trace should be captured without source-level information.
        /// </summary>
        WithoutSource = 1,

#if !SILVERLIGHT
        /// <summary>
        ///     Stack trace should be captured including source-level information such as line numbers.
        /// </summary>
        WithSource = 2,

        /// <summary>
        ///     Capture maximum amount of the stack trace information supported on the plaform.
        /// </summary>
        Max = 2,
#else
    /// <summary>
    /// Capture maximum amount of the stack trace information supported on the plaform.
    /// </summary>
        Max = 1,
#endif
    }
}