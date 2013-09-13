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

using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Planning.Targets;

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Defines the interface for constructor arguments.
    /// </summary>
    public interface IConstructorArgument : IParameter
    {
        /// <summary>
        ///     Determines if the parameter applies to the given target.
        /// </summary>
        /// <remarks>
        ///     Only one parameter may return true.
        /// </remarks>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Tre if the parameter applies in the specified context to the specified target.</returns>
        bool AppliesToTarget(IContext context, ITarget target);
    }
}