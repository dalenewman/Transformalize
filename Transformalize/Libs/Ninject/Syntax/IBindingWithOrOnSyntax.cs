#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWithOrOnSyntax<T> : IBindingWithSyntax<T>, IBindingOnSyntax<T>
    {
    }
}