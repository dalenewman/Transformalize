#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Caching
{
    /// <summary>
    ///     Prunes instances from an <see cref="ICache" /> based on environmental information.
    /// </summary>
    public interface ICachePruner : INinjectComponent
    {
        /// <summary>
        ///     Starts pruning the specified cache based on the rules of the pruner.
        /// </summary>
        /// <param name="cache">The cache that will be pruned.</param>
        void Start(IPruneable cache);

        /// <summary>
        ///     Stops pruning.
        /// </summary>
        void Stop();
    }
}