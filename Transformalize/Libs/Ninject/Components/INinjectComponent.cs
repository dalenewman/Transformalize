#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;

#endregion

namespace Transformalize.Libs.Ninject.Components
{
    /// <summary>
    ///     A component that contributes to the internals of Ninject.
    /// </summary>
    public interface INinjectComponent : IDisposable
    {
        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        INinjectSettings Settings { get; set; }
    }
}