#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.IO;
using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The call site (class name, method name and source information).
    /// </summary>
    [LayoutRenderer("callsite")]
    [ThreadAgnostic]
    public class CallSiteLayoutRenderer : LayoutRenderer, IUsesStackTrace
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CallSiteLayoutRenderer" /> class.
        /// </summary>
        public CallSiteLayoutRenderer()
        {
            ClassName = true;
            MethodName = true;
#if !SILVERLIGHT
            FileName = false;
            IncludeSourcePath = true;
#endif
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to render the class name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool ClassName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to render the method name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool MethodName { get; set; }

#if !SILVERLIGHT
        /// <summary>
        ///     Gets or sets a value indicating whether to render the source file name and line number.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool FileName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include source file path.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool IncludeSourcePath { get; set; }
#endif

        /// <summary>
        ///     Gets the level of stack trace information required by the implementing class.
        /// </summary>
        StackTraceUsage IUsesStackTrace.StackTraceUsage
        {
            get
            {
#if !SILVERLIGHT
                if (FileName)
                {
                    return StackTraceUsage.Max;
                }
#endif

                return StackTraceUsage.WithoutSource;
            }
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
            var frame = logEvent.UserStackFrame;
            if (frame != null)
            {
                var method = frame.GetMethod();
                if (ClassName)
                {
                    if (method.DeclaringType != null)
                    {
                        builder.Append(method.DeclaringType.FullName);
                    }
                    else
                    {
                        builder.Append("<no type>");
                    }
                }

                if (MethodName)
                {
                    if (ClassName)
                    {
                        builder.Append(".");
                    }

                    if (method != null)
                    {
                        builder.Append(method.Name);
                    }
                    else
                    {
                        builder.Append("<no method>");
                    }
                }

#if !SILVERLIGHT
                if (FileName)
                {
                    var fileName = frame.GetFileName();
                    if (fileName != null)
                    {
                        builder.Append("(");
                        if (IncludeSourcePath)
                        {
                            builder.Append(fileName);
                        }
                        else
                        {
                            builder.Append(Path.GetFileName(fileName));
                        }

                        builder.Append(":");
                        builder.Append(frame.GetFileLineNumber());
                        builder.Append(")");
                    }
                }
#endif
            }
        }
    }
}

#endif