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
    ///     Provides an <see cref="AppDomain" /> factory that supports delegated <see cref="AppDomain" /> creation.
    /// </summary>
    internal class DelegateAppDomainFactory : IAppDomainFactory
    {
        #region Fields

        private readonly Func<AppDomain> _factory;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="DelegateAppDomainFactory" />.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        public DelegateAppDomainFactory(Func<AppDomain> factory)
        {
            Contract.Requires(factory != null);

            _factory = factory;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the <see cref="AppDomain" />.
        /// </summary>
        /// <returns>
        ///     The <see cref="AppDomain" /> instance.
        /// </returns>
        public AppDomain CreateAppDomain()
        {
            return _factory();
        }

        #endregion
    }
}