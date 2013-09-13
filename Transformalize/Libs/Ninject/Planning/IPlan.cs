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
using System.Collections.Generic;
using Transformalize.Libs.Ninject.Planning.Directives;

#endregion

namespace Transformalize.Libs.Ninject.Planning
{
    /// <summary>
    ///     Describes the means by which a type should be activated.
    /// </summary>
    public interface IPlan
    {
        /// <summary>
        ///     Gets the type that the plan describes.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Adds the specified directive to the plan.
        /// </summary>
        /// <param name="directive">The directive.</param>
        void Add(IDirective directive);

        /// <summary>
        ///     Determines whether the plan contains one or more directives of the specified type.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>
        ///     <c>True</c> if the plan has one or more directives of the type; otherwise, <c>false</c>.
        /// </returns>
        bool Has<TDirective>() where TDirective : IDirective;

        /// <summary>
        ///     Gets the first directive of the specified type from the plan.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>
        ///     The first directive, or <see langword="null" /> if no matching directives exist.
        /// </returns>
        TDirective GetOne<TDirective>() where TDirective : IDirective;

        /// <summary>
        ///     Gets all directives of the specified type that exist in the plan.
        /// </summary>
        /// <typeparam name="TDirective">The type of directive.</typeparam>
        /// <returns>A series of directives of the specified type.</returns>
        IEnumerable<TDirective> GetAll<TDirective>() where TDirective : IDirective;
    }
}