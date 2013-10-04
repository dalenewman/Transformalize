#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure
{
    /// <summary>
    ///     Indicates that the object has a reference to an <see cref="IKernel" />.
    /// </summary>
    public interface IHaveKernel
    {
        /// <summary>
        ///     Gets the kernel.
        /// </summary>
        IKernel Kernel { get; }
    }
}