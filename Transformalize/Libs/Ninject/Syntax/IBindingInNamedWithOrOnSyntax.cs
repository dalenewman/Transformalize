#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to set the scope, name, or add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingInNamedWithOrOnSyntax<T> :
        IBindingInSyntax<T>,
        IBindingNamedSyntax<T>,
        IBindingWithSyntax<T>,
        IBindingOnSyntax<T>
    {
    }
}