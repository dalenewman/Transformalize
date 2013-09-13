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
using System.Reflection;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Planning.Bindings;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Targets
{
    /// <summary>
    ///     Represents a site on a type where a value will be injected.
    /// </summary>
    public interface ITarget : ICustomAttributeProvider
    {
        /// <summary>
        ///     Gets the type of the target.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Gets the name of the target.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the member that contains the target.
        /// </summary>
        MemberInfo Member { get; }

        /// <summary>
        ///     Gets the constraint defined on the target.
        /// </summary>
        Func<IBindingMetadata, bool> Constraint { get; }

        /// <summary>
        ///     Gets a value indicating whether the target represents an optional dependency.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        ///     Gets a value indicating whether the target has a default value.
        /// </summary>
        bool HasDefaultValue { get; }

        /// <summary>
        ///     Gets the default value for the target.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">If the item does not have a default value.</exception>
        object DefaultValue { get; }

        /// <summary>
        ///     Resolves a value for the target within the specified parent context.
        /// </summary>
        /// <param name="parent">The parent context.</param>
        /// <returns>The resolved value.</returns>
        object ResolveWithin(IContext parent);
    }
}