#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

#endregion

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
    /// <summary>
    ///     Describes the target of a binding.
    /// </summary>
    public enum BindingTarget
    {
        /// <summary>
        ///     Indicates that the binding is from a type to itself.
        /// </summary>
        Self,

        /// <summary>
        ///     Indicates that the binding is from one type to another.
        /// </summary>
        Type,

        /// <summary>
        ///     Indicates that the binding is from a type to a provider.
        /// </summary>
        Provider,

        /// <summary>
        ///     Indicates that the binding is from a type to a callback method.
        /// </summary>
        Method,

        /// <summary>
        ///     Indicates that the binding is from a type to a constant value.
        /// </summary>
        Constant
    }
}