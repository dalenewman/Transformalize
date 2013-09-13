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

using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Planning.Directives;

#endregion

namespace Transformalize.Libs.Ninject.Selection.Heuristics
{
    /// <summary>
    ///     Generates scores for constructors, to determine which is the best one to call during activation.
    /// </summary>
    public interface IConstructorScorer : INinjectComponent
    {
        /// <summary>
        ///     Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        int Score(IContext context, ConstructorInjectionDirective directive);
    }
}