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
using System.Reflection;
using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Targets
{
    /// <summary>
    ///     Represents an injection target for a <see cref="ParameterInfo" />.
    /// </summary>
    public class ParameterTarget : Target<ParameterInfo>
    {
        private readonly Future<object> defaultValue;

        /// <summary>
        ///     Gets the name of the target.
        /// </summary>
        public override string Name
        {
            get { return Site.Name; }
        }

        /// <summary>
        ///     Gets the type of the target.
        /// </summary>
        public override Type Type
        {
            get { return Site.ParameterType; }
        }

// Windows Phone doesn't support default values and returns null instead of DBNull.
#if !WINDOWS_PHONE
        /// <summary>
        ///     Gets a value indicating whether the target has a default value.
        /// </summary>
        public override bool HasDefaultValue
        {
            get { return defaultValue.Value != DBNull.Value; }
        }

        /// <summary>
        ///     Gets the default value for the target.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">If the item does not have a default value.</exception>
        public override object DefaultValue
        {
            get { return HasDefaultValue ? defaultValue.Value : base.DefaultValue; }
        }
#endif

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterTarget" /> class.
        /// </summary>
        /// <param name="method">The method that defines the parameter.</param>
        /// <param name="site">The parameter that this target represents.</param>
        public ParameterTarget(MethodBase method, ParameterInfo site) : base(method, site)
        {
            defaultValue = new Future<object>(() => site.DefaultValue);
        }
    }
}