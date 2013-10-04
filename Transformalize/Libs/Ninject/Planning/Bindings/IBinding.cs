#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
    /// <summary>
    ///     Contains information about a service registration.
    /// </summary>
    public interface IBinding : IBindingConfiguration
    {
        /// <summary>
        ///     Gets the binding configuration.
        /// </summary>
        /// <value>The binding configuration.</value>
        IBindingConfiguration BindingConfiguration { get; }

        /// <summary>
        ///     Gets the service type that is controlled by the binding.
        /// </summary>
        Type Service { get; }
    }
}