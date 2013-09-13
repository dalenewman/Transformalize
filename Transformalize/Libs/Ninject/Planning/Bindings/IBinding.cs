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

#endregion

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
    /// <summary>
    ///     Contains information about a service registration.
    /// </summary>
    public interface IBinding : IBindingConfiguration
    {
        /// <summary>
        ///     Gets the binding configuration.
        /// </summary>
        /// <value>The binding configuration.</value>
        IBindingConfiguration BindingConfiguration { get; }

        /// <summary>
        ///     Gets the service type that is controlled by the binding.
        /// </summary>
        Type Service { get; }
    }
}