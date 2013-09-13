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
using System.Linq;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Activation.Providers;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Bindings.Resolvers
{
    /// <summary>
    /// </summary>
    public class SelfBindingResolver : NinjectComponent, IMissingBindingResolver
    {
        /// <summary>
        ///     Returns any bindings from the specified collection that match the specified service.
        /// </summary>
        /// <param name="bindings">The multimap of all registered bindings.</param>
        /// <param name="request">The service in question.</param>
        /// <returns>The series of matching bindings.</returns>
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
        {
            var service = request.Service;
            if (!TypeIsSelfBindable(service))
            {
                return Enumerable.Empty<IBinding>();
            }
            return new[]
                       {
                           new Binding(service)
                               {
                                   ProviderCallback = StandardProvider.GetCreationCallback(service)
                               }
                       };
        }

        /// <summary>
        ///     Returns a value indicating whether the specified service is self-bindable.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        ///     <see langword="True" /> if the type is self-bindable; otherwise <see langword="false" />.
        /// </returns>
        protected virtual bool TypeIsSelfBindable(Type service)
        {
            return !service.IsInterface
                   && !service.IsAbstract
                   && !service.IsValueType
                   && service != typeof (string)
                   && !service.ContainsGenericParameters;
        }
    }
}