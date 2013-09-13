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
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning;
using Transformalize.Libs.Ninject.Planning.Bindings;

#endregion

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Contains information about the activation of a single instance.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        ///     Gets the kernel that is driving the activation.
        /// </summary>
        IKernel Kernel { get; }

        /// <summary>
        ///     Gets the request.
        /// </summary>
        IRequest Request { get; }

        /// <summary>
        ///     Gets the binding.
        /// </summary>
        IBinding Binding { get; }

        /// <summary>
        ///     Gets or sets the activation plan.
        /// </summary>
        IPlan Plan { get; set; }

        /// <summary>
        ///     Gets the parameters that were passed to manipulate the activation process.
        /// </summary>
        ICollection<IParameter> Parameters { get; }

        /// <summary>
        ///     Gets the generic arguments for the request, if any.
        /// </summary>
        Type[] GenericArguments { get; }

        /// <summary>
        ///     Gets a value indicating whether the request involves inferred generic arguments.
        /// </summary>
        bool HasInferredGenericArguments { get; }

        /// <summary>
        ///     Gets the provider that should be used to create the instance for this context.
        /// </summary>
        /// <returns>The provider that should be used.</returns>
        IProvider GetProvider();

        /// <summary>
        ///     Gets the scope for the context that "owns" the instance activated therein.
        /// </summary>
        /// <returns>The object that acts as the scope.</returns>
        object GetScope();

        /// <summary>
        ///     Resolves this instance for this context.
        /// </summary>
        /// <returns>The resolved instance.</returns>
        object Resolve();
    }
}