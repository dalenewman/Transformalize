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
using Transformalize.Libs.Ninject.Activation.Caching;
using Transformalize.Libs.Ninject.Activation.Strategies;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Drives the activation (injection, etc.) of an instance.
    /// </summary>
    public class Pipeline : NinjectComponent, IPipeline
    {
        /// <summary>
        ///     The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Pipeline" /> class.
        /// </summary>
        /// <param name="strategies">The strategies to execute during activation and deactivation.</param>
        /// <param name="activationCache">The activation cache.</param>
        public Pipeline(IEnumerable<IActivationStrategy> strategies, IActivationCache activationCache)
        {
            Ensure.ArgumentNotNull(strategies, "strategies");
            Strategies = strategies.ToList();
            this.activationCache = activationCache;
        }

        /// <summary>
        ///     Gets the strategies that contribute to the activation and deactivation processes.
        /// </summary>
        public IList<IActivationStrategy> Strategies { get; private set; }

        /// <summary>
        ///     Activates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            if (!activationCache.IsActivated(reference.Instance))
            {
                Strategies.Map(s => s.Activate(context, reference));
            }
        }

        /// <summary>
        ///     Deactivates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            if (!activationCache.IsDeactivated(reference.Instance))
            {
                Strategies.Map(s => s.Deactivate(context, reference));
            }
        }
    }
}