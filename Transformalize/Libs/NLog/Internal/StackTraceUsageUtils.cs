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
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Utilities for dealing with <see cref="StackTraceUsage" /> values.
    /// </summary>
    internal class StackTraceUsageUtils
    {
        internal static StackTraceUsage Max(StackTraceUsage u1, StackTraceUsage u2)
        {
            return (StackTraceUsage) Math.Max((int) u1, (int) u2);
        }
    }
}