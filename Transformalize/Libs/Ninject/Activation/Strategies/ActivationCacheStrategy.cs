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

using Transformalize.Libs.Ninject.Activation.Caching;

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     Adds all activated instances to the activation cache.
    /// </summary>
    public class ActivationCacheStrategy : IActivationStrategy
    {
        /// <summary>
        ///     The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivationCacheStrategy" /> class.
        /// </summary>
        /// <param name="activationCache">The activation cache.</param>
        public ActivationCacheStrategy(IActivationCache activationCache)
        {
            this.activationCache = activationCache;
        }

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>The ninject settings.</value>
        public INinjectSettings Settings { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///     Contributes to the activation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            activationCache.AddActivatedInstance(reference.Instance);
        }

        /// <summary>
        ///     Contributes to the deactivation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            activationCache.AddDeactivatedInstance(reference.Instance);
        }
    }
}