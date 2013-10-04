#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Planning.Bindings;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure
{
    /// <summary>
    ///     Indicates the object has a reference to a <see cref="IBinding" />.
    /// </summary>
    public interface IHaveBindingConfiguration
    {
        /// <summary>
        ///     Gets the binding.
        /// </summary>
        IBindingConfiguration BindingConfiguration { get; }
    }
}