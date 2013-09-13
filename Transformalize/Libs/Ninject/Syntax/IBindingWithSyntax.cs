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
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning.Targets;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to add additional information to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWithSyntax<T> : IBindingSyntax
    {
        /// <summary>
        ///     Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, object value);

        /// <summary>
        ///     Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, object> callback);

        /// <summary>
        ///     Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, ITarget, object> callback);

        /// <summary>
        ///     Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, object value);

        /// <summary>
        ///     Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, object> callback);

        /// <summary>
        ///     Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, ITarget, object> callback);

        /// <summary>
        ///     Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithParameter(IParameter parameter);

        /// <summary>
        ///     Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithMetadata(string key, object value);
    }
}