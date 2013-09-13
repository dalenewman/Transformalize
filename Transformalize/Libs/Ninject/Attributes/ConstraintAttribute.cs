#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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