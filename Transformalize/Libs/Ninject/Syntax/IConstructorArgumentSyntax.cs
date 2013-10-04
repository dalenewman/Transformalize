#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.Ninject.Activation;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Passed to ToConstructor to specify that a constructor value is Injected.
    /// </summary>
    public interface IConstructorArgumentSyntax : IFluentSyntax
    {
        /// <summary>
        ///     Gets the context.
        /// </summary>
        /// <value>The context.</value>
        IContext Context { get; }

        /// <summary>
        ///     Specifies that the argument is injected.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <returns>Not used. This interface has no implementation.</returns>
        T Inject<T>();
    }
}