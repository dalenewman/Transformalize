#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.Ninject.Syntax;

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
    /// <summary>
    ///     The syntax to define bindings.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    public interface IBindingConfigurationSyntax<T> :
        IBindingWhenInNamedWithOrOnSyntax<T>,
        IBindingInNamedWithOrOnSyntax<T>,
        IBindingNamedWithOrOnSyntax<T>,
        IBindingWithOrOnSyntax<T>
    {
    }
}