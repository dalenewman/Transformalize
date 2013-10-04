#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System.Reflection;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;

#endregion

namespace Transformalize.Libs.Ninject.Selection.Heuristics
{
    /// <summary>
    ///     Determines whether members should be injected during activation by checking
    ///     if they are decorated with an injection marker attribute.
    /// </summary>
    public class StandardInjectionHeuristic : NinjectComponent, IInjectionHeuristic
    {
        /// <summary>
        ///     Returns a value indicating whether the specified member should be injected.
        /// </summary>
        /// <param name="member">The member in question.</param>
        /// <returns>
        ///     <c>True</c> if the member should be injected; otherwise <c>false</c>.
        /// </returns>
        public virtual bool ShouldInject(MemberInfo member)
        {
            Ensure.ArgumentNotNull(member, "member");

            var propertyInfo = member as PropertyInfo;

            if (propertyInfo != null)
            {
#if !SILVERLIGHT
                var injectNonPublic = Settings.InjectNonPublic;
#else
                const bool injectNonPublic = false;
#endif
                // !SILVERLIGHT

                var setMethod = propertyInfo.GetSetMethod(injectNonPublic);

                return member.HasAttribute(Settings.InjectAttribute) && setMethod != null;
            }

            return member.HasAttribute(Settings.InjectAttribute);
        }
    }
}