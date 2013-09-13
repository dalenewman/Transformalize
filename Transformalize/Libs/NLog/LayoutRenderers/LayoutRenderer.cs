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
using System.Text;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Render environmental information related to logging events.
    /// </summary>
    [NLogConfigurationItem]
    public abstract class LayoutRenderer : ISupportsInitialize, IRenderable, IDisposable
    {
        private const int MaxInitialRenderBufferLength = 16384;
        private bool isInitialized;
        private int maxRenderedLength;

        /// <summary>
        ///     Gets the logging configuration this target is part of.
        /// </summary>
        protected LoggingConfiguration LoggingConfiguration { get; private set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Renders the the value of layout renderer in the context of the specified log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>String representation of a layout renderer.</returns>
        public string Render(LogEventInfo logEvent)
        {
            var initialLength = maxRenderedLength;
            if (initialLength > MaxInitialRenderBufferLength)
            {
                initialLength = MaxInitialRenderBufferLength;
            }

            var builder = new StringBuilder(initialLength);

            Render(builder, logEvent);
            if (builder.Length > maxRenderedLength)
            {
                maxRenderedLength = builder.Length;
            }

            return builder.ToString();
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
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var lra = (LayoutRendererAttribute) Attribute.GetCustomAttribute(GetType(), typeof (LayoutRendererAttribute));
            if (lra != null)
            {
                return "Layout Renderer: ${" + lra.Name + "}";
            }

            return GetType().Name;
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        internal void Initialize(LoggingConfiguration configuration)
        {
            if (!isInitialized)
            {
                LoggingConfiguration = configuration;
                isInitialized = true;
                InitializeLayoutRenderer();
            }
        }

        /// <summary>
        ///     Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (isInitialized)
            {
                LoggingConfiguration = null;
                isInitialized = false;
                CloseLayoutRenderer();
            }
        }

        internal void Render(StringBuilder builder, LogEventInfo logEvent)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                InitializeLayoutRenderer();
            }

            try
            {
                Append(builder, logEvent);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Warn("Exception in layout renderer: {0}", exception);
            }
        }

        /// <summary>
        ///     Renders the specified environmental information and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected abstract void Append(StringBuilder builder, LogEventInfo logEvent);

        /// <summary>
        ///     Initializes the layout renderer.
        /// </summary>
        protected virtual void InitializeLayoutRenderer()
        {
        }

        /// <summary>
        ///     Closes the layout renderer.
        /// </summary>
        protected virtual void CloseLayoutRenderer()
        {
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
                Close();
            }
        }
    }
}