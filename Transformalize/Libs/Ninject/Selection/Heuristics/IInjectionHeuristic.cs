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