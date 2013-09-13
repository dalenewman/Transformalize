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
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning.Bindings;

#endregion

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Provides a path to resolve instances.
    /// </summary>
    public interface IResolutionRoot : IFluentSyntax
    {
        /// <summary>
        ///     Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        bool CanResolve(IRequest request);

        /// <summary>
        ///     Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">
        ///     if set to <c>true</c> implicit bindings are ignored.
        /// </param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        bool CanResolve(IRequest request, bool ignoreImplicitBindings);

        /// <summary>
        ///     Resolves instances for the specified request. The instances are not actually resolved
        ///     until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>An enumerator of instances that match the request.</returns>
        IEnumerable<object> Resolve(IRequest request);

        /// <summary>
        ///     Creates a request for the specified service.
        /// </summary>
        /// <param name="service">The service that is being requested.</param>
        /// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        /// <param name="parameters">The parameters to pass to the resolution.</param>
        /// <param name="isOptional">
        ///     <c>True</c> if the request is optional; otherwise, <c>false</c>.
        /// </param>
        /// <param name="isUnique">
        ///     <c>True</c> if the request should return a unique result; otherwise, <c>false</c>.
        /// </param>
        /// <returns>The created request.</returns>
        IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique);

        /// <summary>
        ///     Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        ///     <see langword="True" /> if the instance was found and released; otherwise <see langword="false" />.
        /// </returns>
        bool Release(object instance);
    }
}