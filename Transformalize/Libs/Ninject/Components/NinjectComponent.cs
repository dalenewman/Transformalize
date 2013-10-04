#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Infrastructure.Disposal;

#endregion

namespace Transformalize.Libs.Ninject.Components
{
    /// <summary>
    ///     A component that contributes to the internals of Ninject.
    /// </summary>
    public abstract class NinjectComponent : DisposableObject, INinjectComponent
    {
        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        public INinjectSettings Settings { get; set; }
    }
}