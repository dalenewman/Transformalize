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

using System.ComponentModel;
using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Stack trace renderer.
    /// </summary>
    [LayoutRenderer("stacktrace")]
    [ThreadAgnostic]
    public class StackTraceLayoutRenderer : LayoutRenderer, IUsesStackTrace
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StackTraceLayoutRenderer" /> class.
        /// </summary>
        public StackTraceLayoutRenderer()
        {
            Separator = " => ";
            TopFrames = 3;
            Format = StackTraceFormat.Flat;
        }

        /// <summary>
        ///     Gets or sets the output format of the stack trace.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue("Flat")]
        public StackTraceFormat Format { get; set; }

        /// <summary>
        ///     Gets or sets the number of top stack frames to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(3)]
        public int TopFrames { get; set; }

        /// <summary>
        ///     Gets or sets the stack frame separator string.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(" => ")]
        public string Separator { get; set; }

        /// <summary>
        ///     Gets the level of stack trace information required by the implementing class.
        /// </summary>
        /// <value></value>
        StackTraceUsage IUsesStackTrace.StackTraceUsage
        {
            get { return StackTraceUsage.WithoutSource; }
        }

        /// <summary>
        ///     Renders the call site and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var first = true;
            var startingFrame = logEvent.UserStackFrameNumber + TopFrames - 1;
            if (startingFrame >= logEvent.StackTrace.FrameCount)
            {
                startingFrame = logEvent.StackTrace.FrameCount - 1;
            }

            switch (Format)
            {
                case StackTraceFormat.Raw:
                    for (var i = startingFrame; i >= logEvent.UserStackFrameNumber; --i)
                    {
                        var f = logEvent.StackTrace.GetFrame(i);
                        builder.Append(f);
                    }

                    break;

                case StackTraceFormat.Flat:
                    for (var i = startingFrame; i >= logEvent.UserStackFrameNumber; --i)
                    {
                        var f = logEvent.StackTrace.GetFrame(i);
                        if (!first)
                        {
                            builder.Append(Separator);
                        }

                        var type = f.GetMethod().DeclaringType;

                        if (type != null)
                        {
                            builder.Append(type.Name);
                        }
                        else
                        {
                            builder.Append("<no type>");
                        }

                        builder.Append(".");
                        builder.Append(f.GetMethod().Name);
                        first = false;
                    }

                    break;

                case StackTraceFormat.DetailedFlat:
                    for (var i = startingFrame; i >= logEvent.UserStackFrameNumber; --i)
                    {
                        var f = logEvent.StackTrace.GetFrame(i);
                        if (!first)
                        {
                            builder.Append(Separator);
                        }

                        builder.Append("[");
                        builder.Append(f.GetMethod());
                        builder.Append("]");
                        first = false;
                    }

                    break;
            }
        }
    }
}

#endif