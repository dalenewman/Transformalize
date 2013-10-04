#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to define the name of a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingNamedSyntax<T> : IBindingSyntax
    {
        /// <summary>
        ///     Indicates that the binding should be registered with the specified name. Names are not
        ///     necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> Named(string name);
    }
}