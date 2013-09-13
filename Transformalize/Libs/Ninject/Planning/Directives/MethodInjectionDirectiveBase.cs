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

using System.Linq;
using System.Reflection;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Planning.Targets;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Directives
{
    /// <summary>
    ///     Describes the injection of a method or constructor.
    /// </summary>
    public abstract class MethodInjectionDirectiveBase<TMethod, TInjector> : IDirective
        where TMethod : MethodBase
    {
        /// <summary>
        ///     Initializes a new instance of the MethodInjectionDirectiveBase&lt;TMethod, TInjector&gt; class.
        /// </summary>
        /// <param name="method">The method this directive represents.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        protected MethodInjectionDirectiveBase(TMethod method, TInjector injector)
        {
            Ensure.ArgumentNotNull(method, "method");
            Ensure.ArgumentNotNull(injector, "injector");

            Injector = injector;
            Targets = CreateTargetsFromParameters(method);
        }

        /// <summary>
        ///     Gets or sets the injector that will be triggered.
        /// </summary>
        public TInjector Injector { get; private set; }

        /// <summary>
        ///     Gets or sets the targets for the directive.
        /// </summary>
        public ITarget[] Targets { get; private set; }

        /// <summary>
        ///     Creates targets for the parameters of the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The targets for the method's parameters.</returns>
        protected virtual ITarget[] CreateTargetsFromParameters(TMethod method)
        {
            return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).ToArray();
        }
    }
}