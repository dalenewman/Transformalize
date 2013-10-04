#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Infrastructure.Disposal;
using Transformalize.Libs.Ninject.Syntax;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Blocks
{
    /// <summary>
    ///     A block used for deterministic disposal of activated instances. When the block is
    ///     disposed, all instances activated via it will be deactivated.
    /// </summary>
    public interface IActivationBlock : IResolutionRoot, INotifyWhenDisposed
    {
    }
}