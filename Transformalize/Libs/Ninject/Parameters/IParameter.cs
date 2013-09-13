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
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Planning.Targets;

#endregion

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Modifies an activation process in some way.
    /// </summary>
    public interface IParameter : IEquatable<IParameter>
    {
        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets a value indicating whether the parameter should be inherited into child requests.
        /// </summary>
        bool ShouldInherit { get; }

        /// <summary>
        ///     Gets the value for the parameter within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value for the parameter.</returns>
        object GetValue(IContext context, ITarget target);
    }
}