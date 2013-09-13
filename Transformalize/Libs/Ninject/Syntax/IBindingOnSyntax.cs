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
using Transformalize.Libs.Ninject.Activation;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to add additional actions to be performed during activation or deactivation of instances via a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingOnSyntax<T> : IBindingSyntax
    {
        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation(Action<T> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation<TImplementation>(Action<TImplementation> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation(Action<IContext, T> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation<TImplementation>(Action<IContext, TImplementation> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation(Action<T> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<TImplementation> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation(Action<IContext, T> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<IContext, TImplementation> action);
    }
}