#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     A parameter to MethodCall.
    /// </summary>
    [NLogConfigurationItem]
    public class MethodCallParameter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodCallParameter" /> class.
        /// </summary>
        public MethodCallParameter()
        {
            Type = typeof (string);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodCallParameter" /> class.
        /// </summary>
        /// <param name="layout">The layout to use for parameter value.</param>
        public MethodCallParameter(Layout layout)
        {
            Type = typeof (string);
            Layout = layout;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodCallParameter" /> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="layout">The layout.</param>
        public MethodCallParameter(string parameterName, Layout layout)
        {
            Type = typeof (string);
            Name = parameterName;
            Layout = layout;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodCallParameter" /> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="type">The type of the parameter.</param>
        public MethodCallParameter(string name, Layout layout, Type type)
        {
            Type = type;
            Name = name;
            Layout = layout;
        }

        /// <summary>
        ///     Gets or sets the name of the parameter.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the type of the parameter.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Backwards compatibility")]
        public Type Type { get; set; }

        /// <summary>
        ///     Gets or sets the layout that should be use to calcuate the value for the parameter.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [RequiredParameter]
        public Layout Layout { get; set; }

        internal object GetValue(LogEventInfo logEvent)
        {
            return Convert.ChangeType(Layout.Render(logEvent), Type, CultureInfo.InvariantCulture);
        }
    }
}