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

using System;
using System.Diagnostics.Contracts;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines an activator that supports delegated activation.
    /// </summary>
    internal class DelegateActivator : IActivator
    {
        #region Fields

        private readonly Func<InstanceContext, ITemplate> _activator;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="DelegateActivator" />.
        /// </summary>
        /// <param name="activator">The delegated used to create an instance of the template.</param>
        public DelegateActivator(Func<InstanceContext, ITemplate> activator)
        {
            Contract.Requires(activator != null);

            _activator = activator;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the activator.
        /// </summary>
        internal Func<InstanceContext, ITemplate> Activator
        {
            get { return _activator; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an instance of the specifed template.
        /// </summary>
        /// <param name="context">The instance context.</param>
        /// <returns>
        ///     An instance of <see cref="ITemplate" />.
        /// </returns>
        [Pure]
        public ITemplate CreateInstance(InstanceContext context)
        {
            return _activator(context);
        }

        #endregion
    }
}