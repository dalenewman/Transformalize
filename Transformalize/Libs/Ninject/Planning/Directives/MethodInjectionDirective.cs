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

#endregion

namespace Transformalize.Libs.Ninject.Planning.Directives
{
    /// <summary>
    ///     Describes the injection of a method.
    /// </summary>
    public class MethodInjectionDirective : MethodInjectionDirectiveBase<MethodInfo, MethodInjector>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodInjectionDirective" /> class.
        /// </summary>
        /// <param name="method">The method described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public MethodInjectionDirective(MethodInfo method, MethodInjector injector)
            : base(method, injector)
        {
        }
    }
}