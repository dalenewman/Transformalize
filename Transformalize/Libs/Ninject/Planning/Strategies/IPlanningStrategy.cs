#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Strategies
{
    /// <summary>
    ///     Contributes to the generation of a <see cref="IPlan" />.
    /// </summary>
    public interface IPlanningStrategy : INinjectComponent
    {
        /// <summary>
        ///     Contributes to the specified plan.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        void Execute(IPlan plan);
    }
}