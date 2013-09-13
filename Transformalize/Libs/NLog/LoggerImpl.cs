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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Filters;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Implementation of logging engine.
    /// </summary>
    internal static class LoggerImpl
    {
        private const int StackTraceSkipMethods = 0;
        private static readonly Assembly nlogAssembly = typeof (LoggerImpl).Assembly;
        private static readonly Assembly mscorlibAssembly = typeof (string).Assembly;
        private static readonly Assembly systemAssembly = typeof (Debug).Assembly;

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Using 'NLog' in message.")]
        internal static void Write(Type loggerType, TargetWithFilterChain targets, LogEventInfo logEvent, LogFactory factory)
        {
            if (targets == null)
            {
                return;
            }

#if !NET_CF
            var stu = targets.GetStackTraceUsage();

            if (stu != StackTraceUsage.None && !logEvent.HasStackTrace)
            {
                StackTrace stackTrace;
#if !SILVERLIGHT
                stackTrace = new StackTrace(StackTraceSkipMethods, stu == StackTraceUsage.WithSource);
#else
                stackTrace = new StackTrace();
#endif

                var firstUserFrame = FindCallingMethodOnStackTrace(stackTrace, loggerType);

                logEvent.SetStackTrace(stackTrace, firstUserFrame);
            }
#endif

            var originalThreadId = Thread.CurrentThread.ManagedThreadId;
            AsyncContinuation exceptionHandler = ex =>
                                                     {
                                                         if (ex != null)
                                                         {
                                                             if (factory.ThrowExceptions && Thread.CurrentThread.ManagedThreadId == originalThreadId)
                                                             {
                                                                 throw new NLogRuntimeException("Exception occurred in NLog", ex);
                                                             }
                                                         }
                                                     };

            for (var t = targets; t != null; t = t.NextInChain)
            {
                if (!WriteToTargetWithFilterChain(t, logEvent, exceptionHandler))
                {
                    break;
                }
            }
        }

#if !NET_CF
        private static int FindCallingMethodOnStackTrace(StackTrace stackTrace, Type loggerType)
        {
            var firstUserFrame = 0;
            for (var i = 0; i < stackTrace.FrameCount; ++i)
            {
                var frame = stackTrace.GetFrame(i);
                var mb = frame.GetMethod();
                Assembly methodAssembly = null;

                if (mb.DeclaringType != null)
                {
                    methodAssembly = mb.DeclaringType.Assembly;
                }

                if (SkipAssembly(methodAssembly) || mb.DeclaringType == loggerType)
                {
                    firstUserFrame = i + 1;
                }
                else
                {
                    if (firstUserFrame != 0)
                    {
                        break;
                    }
                }
            }

            return firstUserFrame;
        }

        private static bool SkipAssembly(Assembly assembly)
        {
            if (assembly == nlogAssembly)
            {
                return true;
            }

            if (assembly == mscorlibAssembly)
            {
                return true;
            }

            if (assembly == systemAssembly)
            {
                return true;
            }

            return false;
        }
#endif

        private static bool WriteToTargetWithFilterChain(TargetWithFilterChain targetListHead, LogEventInfo logEvent, AsyncContinuation onException)
        {
            var target = targetListHead.Target;
            var result = GetFilterResult(targetListHead.FilterChain, logEvent);

            if ((result == FilterResult.Ignore) || (result == FilterResult.IgnoreFinal))
            {
                if (InternalLogger.IsDebugEnabled)
                {
                    InternalLogger.Debug("{0}.{1} Rejecting message because of a filter.", logEvent.LoggerName, logEvent.Level);
                }

                if (result == FilterResult.IgnoreFinal)
                {
                    return false;
                }

                return true;
            }

            target.WriteAsyncLogEvent(logEvent.WithContinuation(onException));
            if (result == FilterResult.LogFinal)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets the filter result.
        /// </summary>
        /// <param name="filterChain">The filter chain.</param>
        /// <param name="logEvent">The log event.</param>
        /// <returns>The result of the filter.</returns>
        private static FilterResult GetFilterResult(IEnumerable<Filter> filterChain, LogEventInfo logEvent)
        {
            var result = FilterResult.Neutral;

            try
            {
                foreach (var f in filterChain)
                {
                    result = f.GetFilterResult(logEvent);
                    if (result != FilterResult.Neutral)
                    {
                        break;
                    }
                }

                return result;
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Warn("Exception during filter evaluation: {0}", exception);
                return FilterResult.Ignore;
            }
        }
    }
}