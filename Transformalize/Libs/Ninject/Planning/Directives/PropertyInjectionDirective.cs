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

using System.Reflection;
using Transformalize.Libs.Ninject.Injection;
using Transformalize.Libs.Ninject.Planning.Targets;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Directives
{
    /// <summary>
    ///     Describes the injection of a property.
    /// </summary>
    public class PropertyInjectionDirective : IDirective
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyInjectionDirective" /> class.
        /// </summary>
        /// <param name="member">The member the directive describes.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public PropertyInjectionDirective(PropertyInfo member, PropertyInjector injector)
        {
            Injector = injector;
            Target = CreateTarget(member);
        }

        /// <summary>
        ///     Gets or sets the injector that will be triggered.
        /// </summary>
        public PropertyInjector Injector { get; private set; }

        /// <summary>
        ///     Gets or sets the injection target for the directive.
        /// </summary>
        public ITarget Target { get; private set; }

        /// <summary>
        ///     Creates a target for the property.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        /// <returns>The target for the property.</returns>
        protected virtual ITarget CreateTarget(PropertyInfo propertyInfo)
        {
            return new PropertyTarget(propertyInfo);
        }
    }
}