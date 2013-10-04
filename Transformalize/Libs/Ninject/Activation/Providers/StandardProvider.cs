#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using System.Linq;
using System.Reflection;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Introspection;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning;
using Transformalize.Libs.Ninject.Planning.Directives;
using Transformalize.Libs.Ninject.Planning.Targets;
using Transformalize.Libs.Ninject.Selection;
using Transformalize.Libs.Ninject.Selection.Heuristics;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Providers
{
    /// <summary>
    ///     The standard provider for types, which activates instances via a <see cref="IPipeline" />.
    /// </summary>
    public class StandardProvider : IProvider
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StandardProvider" /> class.
        /// </summary>
        /// <param name="type">The type (or prototype) of instances the provider creates.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="constructorScorer">The constructor scorer component.</param>
        public StandardProvider(Type type, IPlanner planner, IConstructorScorer constructorScorer
            )
        {
            Ensure.ArgumentNotNull(type, "type");
            Ensure.ArgumentNotNull(planner, "planner");
            Ensure.ArgumentNotNull(constructorScorer, "constructorScorer");

            Type = type;
            Planner = planner;
            ConstructorScorer = constructorScorer;
        }

        /// <summary>
        ///     Gets or sets the planner component.
        /// </summary>
        public IPlanner Planner { get; private set; }

        /// <summary>
        ///     Gets or sets the selector component.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; private set; }

        /// <summary>
        ///     Gets the type (or prototype) of instances the provider creates.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        ///     Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public virtual object Create(IContext context)
        {
            Ensure.ArgumentNotNull(context, "context");

            if (context.Plan == null)
            {
                context.Plan = Planner.GetPlan(GetImplementationType(context.Request.Service));
            }

            if (!context.Plan.Has<ConstructorInjectionDirective>())
            {
                throw new ActivationException(ExceptionFormatter.NoConstructorsAvailable(context));
            }

            var directives = context.Plan.GetAll<ConstructorInjectionDirective>();
            var bestDirectives = directives
                .GroupBy(option => ConstructorScorer.Score(context, option))
                .OrderByDescending(g => g.Key)
                .First();
            if (bestDirectives.Skip(1).Any())
            {
                throw new ActivationException(ExceptionFormatter.ConstructorsAmbiguous(context, bestDirectives));
            }

            var directive = bestDirectives.Single();
            var arguments = directive.Targets.Select(target => GetValue(context, target)).ToArray();
            return directive.Injector(arguments);
        }

        /// <summary>
        ///     Gets the value to inject into the specified target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value to inject into the specified target.</returns>
        public object GetValue(IContext context, ITarget target)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(target, "target");

            var parameter = context
                .Parameters.OfType<IConstructorArgument>()
                .Where(p => p.AppliesToTarget(context, target)).SingleOrDefault();
            return parameter != null ? parameter.GetValue(context, target) : target.ResolveWithin(context);
        }

        /// <summary>
        ///     Gets the implementation type that the provider will activate an instance of
        ///     for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>The implementation type that will be activated.</returns>
        public Type GetImplementationType(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");
            return Type.ContainsGenericParameters ? Type.MakeGenericType(service.GetGenericArguments()) : Type;
        }

        /// <summary>
        ///     Gets a callback that creates an instance of the <see cref="StandardProvider" />
        ///     for the specified type.
        /// </summary>
        /// <param name="prototype">The prototype the provider instance will create.</param>
        /// <returns>The created callback.</returns>
        public static Func<IContext, IProvider> GetCreationCallback(Type prototype)
        {
            Ensure.ArgumentNotNull(prototype, "prototype");
            return ctx => new StandardProvider(prototype, ctx.Kernel.Components.Get<IPlanner>(), ctx.Kernel.Components.Get<ISelector>().ConstructorScorer);
        }

        /// <summary>
        ///     Gets a callback that creates an instance of the <see cref="StandardProvider" />
        ///     for the specified type and constructor.
        /// </summary>
        /// <param name="prototype">The prototype the provider instance will create.</param>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created callback.</returns>
        public static Func<IContext, IProvider> GetCreationCallback(Type prototype, ConstructorInfo constructor)
        {
            Ensure.ArgumentNotNull(prototype, "prototype");
            return ctx => new StandardProvider(prototype, ctx.Kernel.Components.Get<IPlanner>(), new SpecificConstructorSelector(constructor));
        }
    }
}