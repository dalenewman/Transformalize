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
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Planning.Directives;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     Injects methods on an instance during activation.
    /// </summary>
    public class MethodInjectionStrategy : ActivationStrategy
    {
        /// <summary>
        ///     Injects values into the properties as described by <see cref="MethodInjectionDirective" />s
        ///     contained in the plan.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(reference, "reference");

            foreach (var directive in context.Plan.GetAll<MethodInjectionDirective>())
            {
                var arguments = directive.Targets.Select(target => target.ResolveWithin(context));
                directive.Injector(reference.Instance, arguments.ToArray());
            }
        }
    }
}