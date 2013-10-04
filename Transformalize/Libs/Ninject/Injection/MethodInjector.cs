#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.Ninject.Injection
{
    /// <summary>
    ///     A delegate that can inject values into a method.
    /// </summary>
    public delegate void MethodInjector(object target, params object[] arguments);
}