#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Represents logging target.
    /// </summary>
    [NLogConfigurationItem]
    public abstract class Target : ISupportsInitialize, IDisposable
    {
        private readonly object lockObject = new object();
        private List<Layout> allLayouts;
        private Exception initializeException;

        /// <summary>
        ///     Gets or sets the name of the target.
        /// </summary>
        /// <docgen category='General Options' order='10' />
        public string Name { get; set; }

        /// <summary>
        ///     Gets the object which can be used to synchronize asynchronous operations that must rely on the .
        /// </summary>
        protected object SyncRoot
        {
            get { return lockObject; }
        }

        /// <summary>
        ///     Gets the logging configuration this target is part of.
        /// </summary>
        protected LoggingConfiguration LoggingConfiguration { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the target has been initialized.
        /// </summary>
        protected bool IsInitialized { get; private set; }

        /// <summary>
        ///     Closes the target.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        void ISupportsInitialize.Initialize(LoggingConfiguration configuration)
        {
            Initialize(configuration);
        }

        /// <summary>
        ///     Closes this instance.
        /// </summary>
        void ISupportsInitialize.Close()
        {
            Close();
        }

        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        public void Flush(AsyncContinuation asyncContinuation)
        {
            if (asyncContinuation == null)
            {
                throw new ArgumentNullException("asyncContinuation");
            }

            lock (SyncRoot)
            {
                if (!IsInitialized)
                {
                    asyncContinuation(null);
                    return;
                }

                asyncContinuation = AsyncHelpers.PreventMultipleCalls(asyncContinuation);

                try
                {
                    FlushAsync(asyncContinuation);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    asyncContinuation(exception);
                }
            }
        }

        /// <summary>
        ///     Calls the <see cref="Layout.Precalculate" /> on each volatile layout
        ///     used by this target.
        /// </summary>
        /// <param name="logEvent">
        ///     The log event.
        /// </param>
        public void PrecalculateVolatileLayouts(LogEventInfo logEvent)
        {
            lock (SyncRoot)
            {
                if (IsInitialized)
                {
                    foreach (var l in allLayouts)
                    {
                        l.Precalculate(logEvent);
                    }
                }
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var targetAttribute = (TargetAttribute) Attribute.GetCustomAttribute(GetType(), typeof (TargetAttribute));
            if (targetAttribute != null)
            {
                return targetAttribute.Name + " Target[" + (Name ?? "(unnamed)") + "]";
            }

            return GetType().Name;
        }

        /// <summary>
        ///     Writes the log to the target.
        /// </summary>
        /// <param name="logEvent">Log event to write.</param>
        public void WriteAsyncLogEvent(AsyncLogEventInfo logEvent)
        {
            lock (SyncRoot)
            {
                if (!IsInitialized)
                {
                    logEvent.Continuation(null);
                    return;
                }

                if (initializeException != null)
                {
                    logEvent.Continuation(CreateInitException());
                    return;
                }

                var wrappedContinuation = AsyncHelpers.PreventMultipleCalls(logEvent.Continuation);

                try
                {
                    Write(logEvent.LogEvent.WithContinuation(wrappedContinuation));
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    wrappedContinuation(exception);
                }
            }
        }

        /// <summary>
        ///     Writes the array of log events.
        /// </summary>
        /// <param name="logEvents">The log events.</param>
        public void WriteAsyncLogEvents(params AsyncLogEventInfo[] logEvents)
        {
            lock (SyncRoot)
            {
                if (!IsInitialized)
                {
                    foreach (var ev in logEvents)
                    {
                        ev.Continuation(null);
                    }

                    return;
                }

                if (initializeException != null)
                {
                    foreach (var ev in logEvents)
                    {
                        ev.Continuation(CreateInitException());
                    }

                    return;
                }

                var wrappedEvents = new AsyncLogEventInfo[logEvents.Length];
                for (var i = 0; i < logEvents.Length; ++i)
                {
                    wrappedEvents[i] = logEvents[i].LogEvent.WithContinuation(AsyncHelpers.PreventMultipleCalls(logEvents[i].Continuation));
                }

                try
                {
                    Write(wrappedEvents);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    // in case of synchronous failure, assume that nothing is running asynchronously
                    foreach (var ev in wrappedEvents)
                    {
                        ev.Continuation(exception);
                    }
                }
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        internal void Initialize(LoggingConfiguration configuration)
        {
            lock (SyncRoot)
            {
                LoggingConfiguration = configuration;

                if (!IsInitialized)
                {
                    PropertyHelper.CheckRequiredParameters(this);
                    IsInitialized = true;
                    try
                    {
                        InitializeTarget();
                        initializeException = null;
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        initializeException = exception;
                        InternalLogger.Error("Error initializing target {0} {1}.", this, exception);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        ///     Closes this instance.
        /// </summary>
        internal void Close()
        {
            lock (SyncRoot)
            {
                LoggingConfiguration = null;

                if (IsInitialized)
                {
                    IsInitialized = false;

                    try
                    {
                        if (initializeException == null)
                        {
                            // if Init succeeded, call Close()
                            CloseTarget();
                        }
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        InternalLogger.Error("Error closing target {0} {1}.", this, exception);
                        throw;
                    }
                }
            }
        }

        internal void WriteAsyncLogEvents(AsyncLogEventInfo[] logEventInfos, AsyncContinuation continuation)
        {
            if (logEventInfos.Length == 0)
            {
                continuation(null);
            }
            else
            {
                var wrappedLogEventInfos = new AsyncLogEventInfo[logEventInfos.Length];
                var remaining = logEventInfos.Length;
                for (var i = 0; i < logEventInfos.Length; ++i)
                {
                    var originalContinuation = logEventInfos[i].Continuation;
                    AsyncContinuation wrappedContinuation = ex =>
                                                                {
                                                                    originalContinuation(ex);
                                                                    if (0 == Interlocked.Decrement(ref remaining))
                                                                    {
                                                                        continuation(null);
                                                                    }
                                                                };

                    wrappedLogEventInfos[i] = logEventInfos[i].LogEvent.WithContinuation(wrappedContinuation);
                }

                WriteAsyncLogEvents(wrappedLogEventInfos);
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseTarget();
            }
        }

        /// <summary>
        ///     Initializes the target. Can be used by inheriting classes
        ///     to initialize logging.
        /// </summary>
        protected virtual void InitializeTarget()
        {
            GetAllLayouts();
        }

        /// <summary>
        ///     Closes the target and releases any unmanaged resources.
        /// </summary>
        protected virtual void CloseTarget()
        {
        }

        /// <summary>
        ///     Flush any pending log messages asynchronously (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        protected virtual void FlushAsync(AsyncContinuation asyncContinuation)
        {
            asyncContinuation(null);
        }

        /// <summary>
        ///     Writes logging event to the log target.
        ///     classes.
        /// </summary>
        /// <param name="logEvent">
        ///     Logging event to be written out.
        /// </param>
        protected virtual void Write(LogEventInfo logEvent)
        {
            // do nothing
        }

        /// <summary>
        ///     Writes log event to the log target. Must be overridden in inheriting
        ///     classes.
        /// </summary>
        /// <param name="logEvent">Log event to be written out.</param>
        protected virtual void Write(AsyncLogEventInfo logEvent)
        {
            try
            {
                Write(logEvent.LogEvent);
                logEvent.Continuation(null);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                logEvent.Continuation(exception);
            }
        }

        /// <summary>
        ///     Writes an array of logging events to the log target. By default it iterates on all
        ///     events and passes them to "Write" method. Inheriting classes can use this method to
        ///     optimize batch writes.
        /// </summary>
        /// <param name="logEvents">Logging events to be written out.</param>
        protected virtual void Write(AsyncLogEventInfo[] logEvents)
        {
            for (var i = 0; i < logEvents.Length; ++i)
            {
                Write(logEvents[i]);
            }
        }

        private Exception CreateInitException()
        {
            return new NLogRuntimeException("Target " + this + " failed to initialize.", initializeException);
        }

        private void GetAllLayouts()
        {
            allLayouts = new List<Layout>(ObjectGraphScanner.FindReachableObjects<Layout>(this));
            InternalLogger.Trace("{0} has {1} layouts", this, allLayouts.Count);
        }
    }
}