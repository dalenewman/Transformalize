//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.ComponentModel;
using System.Timers;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Storage
{
    /// <summary>
    /// <para>Represents an <see cref="IConfigurationChangeWatcher"/> that watches a file.</para>
    /// </summary>
    public abstract class ConfigurationChangeWatcher : IConfigurationChangeWatcher
    {
        private static readonly object configurationChangedKey = new object();
        internal static int defaultPollDelayInMilliseconds = 15000;

        private int pollDelayInMilliseconds = defaultPollDelayInMilliseconds;

        private EventHandlerList eventHandlers = new EventHandlerList();
        private DateTime lastWriteTime;
        private ElapsedEventHandler pollTimerHandler;
        private Timer pollTimer;
        private bool polling = false;

        /// <summary>
        /// Sets the default poll delay.
        /// </summary>
        /// <param name="newDefaultPollDelayInMilliseconds">The new default poll.</param>
        public static void SetDefaultPollDelayInMilliseconds(int newDefaultPollDelayInMilliseconds)
        {
            defaultPollDelayInMilliseconds = newDefaultPollDelayInMilliseconds;
        }

        /// <summary>
        /// Reset the default to 15000 millisecond.
        /// </summary>
        public static void ResetDefaultPollDelay()
        {
            defaultPollDelayInMilliseconds = 15000;
        }

        /// <summary>
        /// Sets the poll delay in milliseconds.
        /// </summary>
        /// <param name="newDelayInMilliseconds">
        /// The poll delay in milliseconds.
        /// </param>
        public void SetPollDelayInMilliseconds(int newDelayInMilliseconds)
        {
            pollDelayInMilliseconds = newDelayInMilliseconds;
        }

        /// <summary>
        /// <para>Initialize a new <see cref="ConfigurationChangeWatcher"/> class</para>
        /// </summary>
        public ConfigurationChangeWatcher()
        {

        }

        void pollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
               
                DateTime currentLastWriteTime = GetCurrentLastWriteTime();
                if (currentLastWriteTime != DateTime.MinValue)
                {
                    // might miss a change if a change occurs before it's ran for the first time.
                    if (lastWriteTime.Equals(DateTime.MinValue))
                    {
                        lastWriteTime = currentLastWriteTime;
                    }
                    else
                    {
                        if (lastWriteTime.Equals(currentLastWriteTime) == false)
                        {
                            lastWriteTime = currentLastWriteTime;
                            OnConfigurationChanged();
                        }
                    }
                }
            }
            finally
            {
                if (polling)
                {
                    //auto reset is turned off, therefore we need to restart after the work has been done.
                    pollTimer.Start();
                }
            }
        }

        /// <summary>
        /// <para>
        /// Allows an <see cref="Common.Configuration.Storage.ConfigurationChangeFileWatcher"/> to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="Common.Configuration.Storage.ConfigurationChangeFileWatcher"/> is reclaimed by garbage collection.
        /// </para>
        /// </summary>
        ~ConfigurationChangeWatcher()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Event raised when the underlying persistence mechanism for configuration notices that
        /// the persistent representation of configuration information has changed.
        /// </summary>
        public event ConfigurationChangedEventHandler ConfigurationChanged
        {
            add { eventHandlers.AddHandler(configurationChangedKey, value); }
            remove { eventHandlers.RemoveHandler(configurationChangedKey, value); }
        }

        /// <summary>
        /// <para>Gets the name of the configuration section being watched.</para>
        /// </summary>
        /// <value>
        /// <para>The name of the configuration section being watched.</para>
        /// </value>
        public abstract string SectionName
        {
            get;
        }

        /// <summary>
        /// <para>Starts watching the configuration file.</para>
        /// </summary>
        public void StartWatching()
        {
            
            if (pollTimer == null)
            {
                pollTimer = new Timer();
                pollTimer.Interval = pollDelayInMilliseconds;
                pollTimer.AutoReset = false;

                pollTimerHandler = new ElapsedEventHandler(pollTimer_Elapsed);
                pollTimer.Elapsed += pollTimerHandler;

                lastWriteTime = GetCurrentLastWriteTime();
            }
            polling = true;
            pollTimer.Start();
        }

        /// <summary>
        /// <para>Stops watching the configuration file.</para>
        /// </summary>
        public void StopWatching()
        {
            polling = false;
            if (pollTimer != null)
            {
                pollTimer.Stop();
            }
        }

        /// <summary>
        /// <para>Releases the unmanaged resources used by the <see cref="ConfigurationChangeFileWatcher"/> and optionally releases the managed resources.</para>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <para>Releases the unmanaged resources used by the <see cref="Common.Configuration.Storage.ConfigurationChangeFileWatcher"/> and optionally releases the managed resources.</para>
        /// </summary>
        /// <param name="isDisposing">
        /// <para><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</para>
        /// </param>
        protected virtual void Disposing(bool isDisposing)
        {
            this.Dispose(isDisposing);
        }

        /// <summary>
        /// <para>Releases the unmanaged resources used by the <see cref="Common.Configuration.Storage.ConfigurationChangeFileWatcher"/> and optionally releases the managed resources.</para>
        /// </summary>
        /// <param name="isDisposing">
        /// <para><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</para>
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                eventHandlers.Dispose();
                StopWatching();

                if (pollTimer != null)
                {
                    pollTimer.Elapsed -= pollTimerHandler;
                    pollTimer.Dispose();
                }
            }
        }

        /// <summary>
        /// <para>Raises the <see cref="ConfigurationChanged"/> event.</para>
        /// </summary>
        protected virtual void OnConfigurationChanged()
        {
            ConfigurationChangedEventHandler callbacks = (ConfigurationChangedEventHandler)eventHandlers[configurationChangedKey];
            ConfigurationChangedEventArgs eventData = this.BuildEventData();

            try
            {
                if (callbacks != null)
                {
                    foreach (ConfigurationChangedEventHandler callback in callbacks.GetInvocationList())
                    {
                        if (callback != null)
                        {
                            callback(this, eventData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        private void LogException(Exception e)
        {
            try
            {
            }
            catch
            {
                // Just drop this on the floor. If sending it to the EventLog failed, there is nowhere
                // else for us to send it. Sorry!
            }
        }

        /// <summary>
        /// <para>Returns the <see cref="DateTime"/> of the last change of the information watched</para>
        /// </summary>
        /// <returns>The <see cref="DateTime"/> of the last modificaiton, or <code>DateTime.MinValue</code> if the information can't be retrieved</returns>
        protected abstract DateTime GetCurrentLastWriteTime();


        /// <summary>
        /// Builds the change event data, in a suitable way for the specific watcher implementation
        /// </summary>
        /// <returns>The change event information</returns>
        protected abstract ConfigurationChangedEventArgs BuildEventData();

        /// <summary>
        /// Returns the source name to use when logging events
        /// </summary>
        /// <returns>The event source name</returns>
        protected abstract string GetEventSourceName();
    }
}
