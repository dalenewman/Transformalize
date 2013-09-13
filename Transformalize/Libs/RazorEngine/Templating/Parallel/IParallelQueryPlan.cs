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

using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Libs.RazorEngine.Templating.Parallel
{
    /// <summary>
    ///     Defines the required contract for implementing a parallel query plan.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface IParallelQueryPlan<T>
    {
        #region Methods

        /// <summary>
        ///     Creates a parallel query for the specified source.
        /// </summary>
        /// <param name="source">The source enumerable.</param>
        /// <returns>The parallel query.</returns>
        ParallelQuery<T> CreateQuery(IEnumerable<T> source);

        #endregion
    }
}