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
using System.Reflection;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Selection.Heuristics;

#endregion

namespace Transformalize.Libs.Ninject.Selection
{
    /// <summary>
    ///     Selects members for injection.
    /// </summary>
    public interface ISelector : INinjectComponent
    {
        /// <summary>
        ///     Gets or sets the constructor scorer.
        /// </summary>
        IConstructorScorer ConstructorScorer { get; set; }

        /// <summary>
        ///     Gets the heuristics used to determine which members should be injected.
        /// </summary>
        ICollection<IInjectionHeuristic> InjectionHeuristics { get; }

        /// <summary>
        ///     Selects the constructor to call on the specified type, by using the constructor scorer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     The selected constructor, or <see langword="null" /> if none were available.
        /// </returns>
        IEnumerable<ConstructorInfo> SelectConstructorsForInjection(Type type);

        /// <summary>
        ///     Selects properties that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected properties.</returns>
        IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type);

        /// <summary>
        ///     Selects methods that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected methods.</returns>
        IEnumerable<MethodInfo> SelectMethodsForInjection(Type type);
    }
}