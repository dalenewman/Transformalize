#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using Transformalize.Libs.Ninject.Planning.Bindings;

#endregion

namespace Transformalize.Libs.Ninject.Attributes
{
    /// <summary>
    ///     Defines a constraint on the decorated member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class ConstraintAttribute : Attribute
    {
        /// <summary>
        ///     Determines whether the specified binding metadata matches the constraint.
        /// </summary>
        /// <param name="metadata">The metadata in question.</param>
        /// <returns>
        ///     <c>True</c> if the metadata matches; otherwise <c>false</c>.
        /// </returns>
        public abstract bool Matches(IBindingMetadata metadata);
    }
}