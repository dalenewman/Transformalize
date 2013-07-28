using System;

namespace Transformalize.Libs.RazorEngine.Core.Templating
{
    /// <summary>
    /// Defines the required contract for implementing an <see cref="AppDomain"/> factory.
    /// </summary>
    public interface IAppDomainFactory
    {
        #region Methods
        /// <summary>
        /// Creates the <see cref="AppDomain"/>.
        /// </summary>
        /// <returns>The <see cref="AppDomain"/> instance.</returns>
        AppDomain CreateAppDomain();
        #endregion
    }
}
