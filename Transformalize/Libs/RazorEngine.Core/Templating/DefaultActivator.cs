using System;
using System.Diagnostics.Contracts;

namespace Transformalize.Libs.RazorEngine.Core.Templating
{
    /// <summary>
    /// Provides a default implementation of an <see cref="IActivator"/>.
    /// </summary>
    internal class DefaultActivator : IActivator
    {
        #region Methods
        /// <summary>
        /// Creates an instance of the specifed template.
        /// </summary>
        /// <param name="context">The instance context.</param>
        /// <returns>An instance of <see cref="ITemplate"/>.</returns>
        [Pure]
        public ITemplate CreateInstance(InstanceContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return context.Loader.CreateInstance(context.TemplateType);
        }
        #endregion
    }
}