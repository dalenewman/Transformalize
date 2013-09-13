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
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Disposal;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning.Bindings;
using Transformalize.Libs.Ninject.Syntax;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Blocks
{
    /// <summary>
    ///     A block used for deterministic disposal of activated instances. When the block is
    ///     disposed, all instances activated via it will be deactivated.
    /// </summary>
    public class ActivationBlock : DisposableObject, IActivationBlock
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivationBlock" /> class.
        /// </summary>
        /// <param name="parent">The parent resolution root.</param>
        public ActivationBlock(IResolutionRoot parent)
        {
            Ensure.ArgumentNotNull(parent, "parent");
            Parent = parent;
        }

        /// <summary>
        ///     Gets or sets the parent resolution root (usually the kernel).
        /// </summary>
        public IResolutionRoot Parent { get; private set; }

        /// <summary>
        ///     Occurs when the object is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        ///     Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");
            return Parent.CanResolve(request);
        }

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
        public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            Ensure.ArgumentNotNull(request, "request");
            return Parent.CanResolve(request, ignoreImplicitBindings);
        }

        /// <summary>
        ///     Resolves instances for the specified request. The instances are not actually resolved
        ///     until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>An enumerator of instances that match the request.</returns>
        public IEnumerable<object> Resolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");
            return Parent.Resolve(request);
        }

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
        public virtual IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(parameters, "parameters");
            return new Request(service, constraint, parameters, () => this, isOptional, isUnique);
        }

        /// <summary>
        ///     Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        ///     <see langword="True" /> if the instance was found and released; otherwise <see langword="false" />.
        /// </returns>
        /// <remarks></remarks>
        public bool Release(object instance)
        {
            return Parent.Release(instance);
        }

        /// <summary>
        ///     Releases resources held by the object.
        /// </summary>
        public override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    var evt = Disposed;
                    if (evt != null) evt(this, EventArgs.Empty);
                    Disposed = null;
                }

                base.Dispose(disposing);
            }
        }
    }
}