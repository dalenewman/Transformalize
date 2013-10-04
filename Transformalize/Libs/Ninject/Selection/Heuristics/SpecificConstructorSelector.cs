#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Reflection;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Planning.Directives;

namespace Transformalize.Libs.Ninject.Selection.Heuristics
{
    /// <summary>
    ///     Constructor selector that selects the constructor matching the one passed to the constructor.
    /// </summary>
    public class SpecificConstructorSelector : NinjectComponent, IConstructorScorer
    {
        private readonly ConstructorInfo constructorInfo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpecificConstructorSelector" /> class.
        /// </summary>
        /// <param name="constructorInfo">The constructor info of the constructor that shall be selected.</param>
        public SpecificConstructorSelector(ConstructorInfo constructorInfo)
        {
            this.constructorInfo = constructorInfo;
        }

        /// <summary>
        ///     Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        public virtual int Score(IContext context, ConstructorInjectionDirective directive)
        {
            return directive.Constructor.Equals(constructorInfo) ? 1 : 0;
        }
    }
}