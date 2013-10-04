#region License
// /*
// See license included in this library folder.
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