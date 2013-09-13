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
using System.Collections;
using System.Linq;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning.Directives;
using Transformalize.Libs.Ninject.Planning.Targets;

namespace Transformalize.Libs.Ninject.Selection.Heuristics
{
    /// <summary>
    ///     Scores constructors by either looking for the existence of an injection marker
    ///     attribute, or by counting the number of parameters.
    /// </summary>
    public class StandardConstructorScorer : NinjectComponent, IConstructorScorer
    {
        /// <summary>
        ///     Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        public virtual int Score(IContext context, ConstructorInjectionDirective directive)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(directive, "constructor");

            if (directive.Constructor.HasAttribute(Settings.InjectAttribute))
            {
                return int.MaxValue;
            }

            var score = 1;
            foreach (var target in directive.Targets)
            {
                if (ParameterExists(context, target))
                {
                    score++;
                    continue;
                }

                if (BindingExists(context, target))
                {
                    score++;
                    continue;
                }

                score++;
                if (score > 0)
                {
                    score += int.MinValue;
                }
            }

            return score;
        }

        /// <summary>
        ///     Checkes whether a binding exists for a given target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Whether a binding exists for the target in the given context.</returns>
        protected virtual bool BindingExists(IContext context, ITarget target)
        {
            return BindingExists(context.Kernel, context, target);
        }

        /// <summary>
        ///     Checkes whether a binding exists for a given target on the specified kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Whether a binding exists for the target in the given context.</returns>
        protected virtual bool BindingExists(IKernel kernel, IContext context, ITarget target)
        {
            var targetType = GetTargetType(target);
            return kernel.GetBindings(targetType).Any(b => !b.IsImplicit)
                   || target.HasDefaultValue;
        }

        private Type GetTargetType(ITarget target)
        {
            var targetType = target.Type;
            if (targetType.IsArray)
            {
                targetType = targetType.GetElementType();
            }

            if (targetType.IsGenericType && targetType.GetInterfaces().Any(type => type == typeof (IEnumerable)))
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            return targetType;
        }

        /// <summary>
        ///     Checks whether any parameters exist for the geiven target..
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>Whether a parameter exists for the target in the given context.</returns>
        protected virtual bool ParameterExists(IContext context, ITarget target)
        {
            return context
                .Parameters.OfType<IConstructorArgument>()
                .Any(parameter => parameter.AppliesToTarget(context, target));
        }
    }
}