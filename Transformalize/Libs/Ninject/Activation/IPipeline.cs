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

using System.Collections.Generic;
using Transformalize.Libs.Ninject.Activation.Strategies;
using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Drives the activation (injection, etc.) of an instance.
    /// </summary>
    public interface IPipeline : INinjectComponent
    {
        /// <summary>
        ///     Gets the strategies that contribute to the activation and deactivation processes.
        /// </summary>
        IList<IActivationStrategy> Strategies { get; }

        /// <summary>
        ///     Activates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        void Activate(IContext context, InstanceReference reference);

        /// <summary>
        ///     Deactivates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        void Deactivate(IContext context, InstanceReference reference);
    }
}