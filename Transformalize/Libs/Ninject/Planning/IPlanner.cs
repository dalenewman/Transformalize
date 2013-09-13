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
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Planning.Strategies;

#endregion

namespace Transformalize.Libs.Ninject.Planning
{
    /// <summary>
    ///     Generates plans for how to activate instances.
    /// </summary>
    public interface IPlanner : INinjectComponent
    {
        /// <summary>
        ///     Gets the strategies that contribute to the planning process.
        /// </summary>
        IList<IPlanningStrategy> Strategies { get; }

        /// <summary>
        ///     Gets or creates an activation plan for the specified type.
        /// </summary>
        /// <param name="type">The type for which a plan should be created.</param>
        /// <returns>The type's activation plan.</returns>
        IPlan GetPlan(Type type);
    }
}