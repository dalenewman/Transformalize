#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.Layouts
{
    /// <summary>
    ///     Abstract interface that layouts must implement.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Few people will see this conflict.")]
    [NLogConfigurationItem]
    public abstract class Layout : ISupportsInitialize, IRenderable
    {
        private bool isInitialized;
        private bool threadAgnostic;

        /// <summary>
        ///     Gets a value indicating whether this layout is thread-agnostic (can be rendered on any thread).
        /// </summary>
        /// <remarks>
        ///     Layout is thread-agnostic if it has been marked with [ThreadAgnostic] attribute and all its children are
        ///     like that as well.
        ///     Thread-agnostic layouts only use contents of <see cref="LogEventInfo" /> for its output.
        /// </remarks>
        internal bool IsThreadAgnostic
        {
            get { return threadAgnostic; }
        }

        /// <summary>
        ///     Gets the logging configuration this target is part of.
        /// </summary>
        protected LoggingConfiguration LoggingConfiguration { get; private set; }

        /// <summary>
        ///     Renders the event info in layout.
        /// </summary>
        /// <param name="logEvent">The event info.</param>
        /// <returns>String representing log event.</returns>
        public string Render(LogEventInfo logEvent)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                InitializeLayout();
            }

            return GetFormattedMessage(logEvent);
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
        ///     Converts a given text to a <see cref="Layout" />.
        /// </summary>
        /// <param name="text">Text to be converted.</param>
        /// <returns>
        ///     <see cref="SimpleLayout" /> object represented by the text.
        /// </returns>
        public static implicit operator Layout([Localizable(false)] string text)
        {
            return FromString(text);
        }

        /// <summary>
        ///     Implicitly converts the specified string to a <see cref="SimpleLayout" />.
        /// </summary>
        /// <param name="layoutText">The layout string.</param>
        /// <returns>
        ///     Instance of <see cref="SimpleLayout" />.
        /// </returns>
        public static Layout FromString(string layoutText)
        {
            return FromString(layoutText, ConfigurationItemFactory.Default);
        }

        /// <summary>
        ///     Implicitly converts the specified string to a <see cref="SimpleLayout" />.
        /// </summary>
        /// <param name="layoutText">The layout string.</param>
        /// <param name="configurationItemFactory">The NLog factories to use when resolving layout renderers.</param>
        /// <returns>
        ///     Instance of <see cref="SimpleLayout" />.
        /// </returns>
        public static Layout FromString(string layoutText, ConfigurationItemFactory configurationItemFactory)
        {
            return new SimpleLayout(layoutText, configurationItemFactory);
        }

        /// <summary>
        ///     Precalculates the layout for the specified log event and stores the result
        ///     in per-log event cache.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <remarks>
        ///     Calling this method enables you to store the log event in a buffer
        ///     and/or potentially evaluate it in another thread even though the
        ///     layout may contain thread-dependent renderer.
        /// </remarks>
        public virtual void Precalculate(LogEventInfo logEvent)
        {
            if (!threadAgnostic)
            {
                Render(logEvent);
            }
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

                // determine whether the layout is thread-agnostic
                // layout is thread agnostic if it is thread-agnostic and 
                // all its nested objects are thread-agnostic.
                threadAgnostic = true;
                foreach (var item in ObjectGraphScanner.FindReachableObjects<object>(this))
                {
                    if (!item.GetType().IsDefined(typeof (ThreadAgnosticAttribute), true))
                    {
                        threadAgnostic = false;
                        break;
                    }
                }

                InitializeLayout();
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
                CloseLayout();
            }
        }

        /// <summary>
        ///     Initializes the layout.
        /// </summary>
        protected virtual void InitializeLayout()
        {
        }

        /// <summary>
        ///     Closes the layout.
        /// </summary>
        protected virtual void CloseLayout()
        {
        }

        /// <summary>
        ///     Renders the layout for the specified logging event by invoking layout renderers.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        /// <returns>The rendered layout.</returns>
        protected abstract string GetFormattedMessage(LogEventInfo logEvent);
    }
}