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
    ///     Used to define the scope in which instances activated via a binding should be re-used.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingInSyntax<T> : IBindingSyntax
    {
        /// <summary>
        ///     Indicates that only a single instance of the binding should be created, and then
        ///     should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        IBindingNamedWithOrOnSyntax<T> InSingletonScope();

        /// <summary>
        ///     Indicates that instances activated via the binding should not be re-used, nor have
        ///     their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        IBindingNamedWithOrOnSyntax<T> InTransientScope();

        /// <summary>
        ///     Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        IBindingNamedWithOrOnSyntax<T> InThreadScope();

        /// <summary>
        ///     Indicates that instances activated via the binding should be re-used as long as the object
        ///     returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingNamedWithOrOnSyntax<T> InScope(Func<IContext, object> scope);
    }
}