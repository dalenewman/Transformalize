#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Provides instances ot the type T
    /// </summary>
    /// <typeparam name="T">The type provides by this implementation.</typeparam>
    public interface IProvider<T> : IProvider
    {
    }
}