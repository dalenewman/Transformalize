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

using System.Reflection;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Planning.Directives;

namespace Transformalize.Libs.Ninject.Selection.Heuristics
{
    /// <summary>
    ///     Constructor selector that selects the constructor matching the one passed to the constructor.
    /// </summary>
    public class SpecificConstructorSelector : NinjectComponent, IConstructorScorer
    {
        private readonly ConstructorInfo constructorInfo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpecificConstructorSelector" /> class.
        /// </summary>
        /// <param name="constructorInfo">The constructor info of the constructor that shall be selected.</param>
        public SpecificConstructorSelector(ConstructorInfo constructorInfo)
        {
            this.constructorInfo = constructorInfo;
        }

        /// <summary>
        ///     Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        public virtual int Score(IContext context, ConstructorInjectionDirective directive)
        {
            return directive.Constructor.Equals(constructorInfo) ? 1 : 0;
        }
    }
}