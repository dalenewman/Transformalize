#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.Ninject.Infrastructure;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to define a basic binding syntax builder.
    /// </summary>
    public interface IBindingSyntax : IHaveBindingConfiguration, IHaveKernel, IFluentSyntax
    {
    }
}