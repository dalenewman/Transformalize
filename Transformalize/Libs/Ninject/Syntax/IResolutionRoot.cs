#region License
// /*
// See license included in this library folder.
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