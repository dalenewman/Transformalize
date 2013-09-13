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

using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Allows components to request stack trace information to be provided in the <see cref="LogEventInfo" />.
    /// </summary>
    internal interface IUsesStackTrace
    {
        /// <summary>
        ///     Gets the level of stack trace information required by the implementing class.
        /// </summary>
        StackTraceUsage StackTraceUsage { get; }
    }
}