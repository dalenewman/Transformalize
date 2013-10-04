#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.Ninject.Injection
{
    /// <summary>
    ///     A delegate that can inject values into a constructor.
    /// </summary>
    public delegate object ConstructorInjector(params object[] arguments);
}