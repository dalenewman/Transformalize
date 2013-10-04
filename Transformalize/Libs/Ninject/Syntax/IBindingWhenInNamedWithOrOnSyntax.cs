#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Diagnostics.CodeAnalysis;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to set the condition, scope, name, or add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
    public interface IBindingWhenInNamedWithOrOnSyntax<T> : IBindingWhenSyntax<T>,
                                                            IBindingInSyntax<T>,
                                                            IBindingNamedSyntax<T>,
                                                            IBindingWithSyntax<T>,
                                                            IBindingOnSyntax<T>
    {
    }
}