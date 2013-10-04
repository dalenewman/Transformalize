#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Planning.Directives;

#endregion

namespace Transformalize.Libs.Ninject.Selection.Heuristics
{
    /// <summary>
    ///     Generates scores for constructors, to determine which is the best one to call during activation.
    /// </summary>
    public interface IConstructorScorer : INinjectComponent
    {
        /// <summary>
        ///     Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        int Score(IContext context, ConstructorInjectionDirective directive);
    }
}