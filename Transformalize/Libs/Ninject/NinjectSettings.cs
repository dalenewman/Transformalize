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
using Transformalize.Libs.Ninject.Attributes;
using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject
{
    /// <summary>
    ///     Contains configuration options for Ninject.
    /// </summary>
    public class NinjectSettings : INinjectSettings
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        /// <summary>
        ///     Gets or sets the attribute that indicates that a member should be injected.
        /// </summary>
        public Type InjectAttribute
        {
            get { return Get("InjectAttribute", typeof (InjectAttribute)); }
            set { Set("InjectAttribute", value); }
        }

        /// <summary>
        ///     Gets or sets the interval at which the GC should be polled.
        /// </summary>
        public TimeSpan CachePruningInterval
        {
            get { return Get("CachePruningInterval", TimeSpan.FromSeconds(30)); }
            set { Set("CachePruningInterval", value); }
        }

        /// <summary>
        ///     Gets or sets the default scope callback.
        /// </summary>
        public Func<IContext, object> DefaultScopeCallback
        {
            get { return Get("DefaultScopeCallback", StandardScopeCallbacks.Transient); }
            set { Set("DefaultScopeCallback", value); }
        }

#if !NO_ASSEMBLY_SCANNING
        /// <summary>
        ///     Gets or sets a value indicating whether the kernel should automatically load extensions at startup.
        /// </summary>
        public bool LoadExtensions
        {
            get { return Get("LoadExtensions", true); }
            set { Set("LoadExtensions", value); }
        }

        /// <summary>
        ///     Gets or sets the paths that should be searched for extensions.
        /// </summary>
        public string[] ExtensionSearchPatterns
        {
            get { return Get("ExtensionSearchPatterns", new[] {"Ninject.Extensions.*.dll", "Ninject.Web*.dll"}); }
            set { Set("ExtensionSearchPatterns", value); }
        }
#endif
        //!NO_ASSEMBLY_SCANNING

#if !NO_LCG
        /// <summary>
        ///     Gets a value indicating whether Ninject should use reflection-based injection instead of
        ///     the (usually faster) lightweight code generation system.
        /// </summary>
        public bool UseReflectionBasedInjection
        {
            get { return Get("UseReflectionBasedInjection", false); }
            set { Set("UseReflectionBasedInjection", value); }
        }
#endif
        //!NO_LCG

#if !SILVERLIGHT
        /// <summary>
        ///     Gets a value indicating whether Ninject should inject non public members.
        /// </summary>
        public bool InjectNonPublic
        {
            get { return Get("InjectNonPublic", false); }
            set { Set("InjectNonPublic", value); }
        }

        /// <summary>
        ///     Gets a value indicating whether Ninject should inject private properties of base classes.
        /// </summary>
        /// <remarks>
        ///     Activating this setting has an impact on the performance. It is recomended not
        ///     to use this feature and use constructor injection instead.
        /// </remarks>
        public bool InjectParentPrivateProperties
        {
            get { return Get("InjectParentPrivateProperties", false); }
            set { Set("InjectParentPrivateProperties", value); }
        }
#endif
        //!SILVERLIGHT

        /// <summary>
        ///     Gets or sets a value indicating whether the activation cache is disabled.
        ///     If the activation cache is disabled less memory is used. But in some cases
        ///     instances are activated or deactivated multiple times. e.g. in the following scenario:
        ///     Bind{A}().ToSelf();
        ///     Bind{IA}().ToMethod(ctx =&gt; kernel.Get{IA}();
        /// </summary>
        /// <value>
        ///     <c>true</c> if activation cache is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool ActivationCacheDisabled
        {
            get { return Get("ActivationCacheDisabled", false); }
            set { Set("ActivationCacheDisabled", value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether Null is a valid value for injection.
        ///     By default this is disabled and whenever a provider returns null an exception is thrown.
        /// </summary>
        /// <value>
        ///     <c>true</c> if null is allowed as injected value otherwise false.
        /// </value>
        public bool AllowNullInjection
        {
            get { return Get("AllowNullInjection", false); }
            set { Set("AllowNullInjection", value); }
        }

        /// <summary>
        ///     Gets the value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="defaultValue">The value to return if no setting is available.</param>
        /// <returns>The value, or the default value if none was found.</returns>
        public T Get<T>(string key, T defaultValue)
        {
            object value;
            return _values.TryGetValue(key, out value) ? (T) value : defaultValue;
        }

        /// <summary>
        ///     Sets the value for the specified key.
        /// </summary>
        /// <param name="key">The setting's key.</param>
        /// <param name="value">The setting's value.</param>
        public void Set(string key, object value)
        {
            _values[key] = value;
        }
    }
}