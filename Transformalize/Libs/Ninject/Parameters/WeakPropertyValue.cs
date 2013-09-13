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

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Overrides the injected value of a property.
    ///     Keeps a weak reference to the value.
    /// </summary>
    public class WeakPropertyValue : Parameter, IPropertyValue
    {
        private readonly WeakReference weakReference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakPropertyValue" /> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public WeakPropertyValue(string name, object value)
            : base(name, (object) null, false)
        {
            weakReference = new WeakReference(value);
            ValueCallback = (ctx, target) => weakReference.Target;
        }
    }
}