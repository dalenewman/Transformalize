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
using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Injection
{
    /// <summary>
    ///     Creates injectors from members.
    /// </summary>
    public interface IInjectorFactory : INinjectComponent
    {
        /// <summary>
        ///     Gets or creates an injector for the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created injector.</returns>
        ConstructorInjector Create(ConstructorInfo constructor);

        /// <summary>
        ///     Gets or creates an injector for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The created injector.</returns>
        PropertyInjector Create(PropertyInfo property);

        /// <summary>
        ///     Gets or creates an injector for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The created injector.</returns>
        MethodInjector Create(MethodInfo method);
    }
}