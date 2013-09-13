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
using System.Collections.Generic;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Bindings.Resolvers
{
    /// <summary>
    ///     Contains logic about which bindings to use for a given service request
    ///     when other attempts have failed.
    /// </summary>
    public interface IMissingBindingResolver : INinjectComponent
    {
        /// <summary>
        ///     Returns any bindings from the specified collection that match the specified request.
        /// </summary>
        /// <param name="bindings">The multimap of all registered bindings.</param>
        /// <param name="request">The request in question.</param>
        /// <returns>The series of matching bindings.</returns>
        IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request);
    }
}