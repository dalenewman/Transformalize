#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Planning.Targets;

#endregion

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Overrides the injected value of a property.
    /// </summary>
    public class PropertyValue : Parameter, IPropertyValue
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyValue" /> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public PropertyValue(string name, object value) : base(name, value, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyValue" /> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        public PropertyValue(string name, Func<IContext, object> valueCallback) : base(name, valueCallback, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyValue" /> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        public PropertyValue(string name, Func<IContext, ITarget, object> valueCallback) : base(name, valueCallback, false)
        {
        }
    }
}