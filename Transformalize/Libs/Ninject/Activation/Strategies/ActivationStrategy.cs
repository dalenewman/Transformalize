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

using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     Contributes to a <see cref="IPipeline" />, and is called during the activation
    ///     and deactivation of an instance.
    /// </summary>
    public abstract class ActivationStrategy : NinjectComponent, IActivationStrategy
    {
        /// <summary>
        ///     Contributes to the activation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public virtual void Activate(IContext context, InstanceReference reference)
        {
        }

        /// <summary>
        ///     Contributes to the deactivation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public virtual void Deactivate(IContext context, InstanceReference reference)
        {
        }
    }
}