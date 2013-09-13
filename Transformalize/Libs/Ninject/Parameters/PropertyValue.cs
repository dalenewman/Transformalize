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