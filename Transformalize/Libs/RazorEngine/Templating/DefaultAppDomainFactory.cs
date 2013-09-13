using System;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Provides a default implementation of an <see cref="AppDomain" /> factory.
    /// </summary>
    public class DefaultAppDomainFactory : IAppDomainFactory
    {
        #region Methods

        /// <summary>
        ///     Creates the <see cref="AppDomain" />.
        /// </summary>
        /// <returns>
        ///     The <see cref="AppDomain" /> instance.
        /// </returns>
        public AppDomain CreateAppDomain()
        {
            AppDomain current = AppDomain.CurrentDomain;
            AppDomain domain = AppDomain.CreateDomain("RazorHost", current.Evidence, current.SetupInformation);

            return domain;
        }

        #endregion
    }
}