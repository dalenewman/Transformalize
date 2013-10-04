#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System.Reflection;
using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Selection.Heuristics
{
    /// <summary>
    ///     Determines whether members should be injected during activation.
    /// </summary>
    public interface IInjectionHeuristic : INinjectComponent
    {
        /// <summary>
        ///     Returns a value indicating whether the specified member should be injected.
        /// </summary>
        /// <param name="member">The member in question.</param>
        /// <returns>
        ///     <c>True</c> if the member should be injected; otherwise <c>false</c>.
        /// </returns>
        bool ShouldInject(MemberInfo member);
    }
}