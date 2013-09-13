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
    ///     Used to define the conditions under which a binding should be used.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWhenSyntax<T> : IBindingSyntax
    {
        /// <summary>
        ///     Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> When(Func<IRequest, bool> condition);

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto<TParent>();

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(Type parent);

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified types.
        ///     Types that derive from one of the specified types are considered as valid targets.
        ///     Should match at lease one of the targets.
        /// </summary>
        /// <param name="parents">The types to match.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(params Type[] parents);

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     The type must match exactly the specified type. Types that derive from the specified type
        ///     will not be considered as valid target.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto<TParent>();

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     The type must match exactly the specified type. Types that derive from the specified type
        ///     will not be considered as valid target.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto(Type parent);

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     The type must match one of the specified types exactly. Types that derive from one of the specified types
        ///     will not be considered as valid target.
        ///     Should match at least one of the specified targets
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto(params Type[] parents);

        /// <summary>
        ///     Indicates that the binding should be used only when the class being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenClassHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        ///     Indicates that the binding should be used only when the member being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenMemberHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        ///     Indicates that the binding should be used only when the target being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenTargetHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        ///     Indicates that the binding should be used only when the class being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenClassHas(Type attributeType);

        /// <summary>
        ///     Indicates that the binding should be used only when the member being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenMemberHas(Type attributeType);

        /// <summary>
        ///     Indicates that the binding should be used only when the target being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenTargetHas(Type attributeType);

        /// <summary>
        ///     Indicates that the binding should be used only when the service is being requested
        ///     by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenParentNamed(string name);

        /// <summary>
        ///     Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        [Obsolete("Use WhenAnyAncestorNamed(string name)")]
        IBindingInNamedWithOrOnSyntax<T> WhenAnyAnchestorNamed(string name);

        /// <summary>
        ///     Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorNamed(string name);

        /// <summary>
        ///     Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorNamed(string name);

        /// <summary>
        ///     Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorMatches(Predicate<IContext> predicate);

        /// <summary>
        ///     Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorMatches(Predicate<IContext> predicate);
    }
}