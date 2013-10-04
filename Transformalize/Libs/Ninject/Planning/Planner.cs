#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;
using Transformalize.Libs.Ninject.Planning.Strategies;

namespace Transformalize.Libs.Ninject.Planning
{
    /// <summary>
    ///     Generates plans for how to activate instances.
    /// </summary>
    public class Planner : NinjectComponent, IPlanner
    {
        private readonly ReaderWriterLock plannerLock = new ReaderWriterLock();
        private readonly Dictionary<Type, IPlan> plans = new Dictionary<Type, IPlan>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Planner" /> class.
        /// </summary>
        /// <param name="strategies">The strategies to execute during planning.</param>
        public Planner(IEnumerable<IPlanningStrategy> strategies)
        {
            Ensure.ArgumentNotNull(strategies, "strategies");
            Strategies = strategies.ToList();
        }

        /// <summary>
        ///     Gets the strategies that contribute to the planning process.
        /// </summary>
        public IList<IPlanningStrategy> Strategies { get; private set; }

        /// <summary>
        ///     Gets or creates an activation plan for the specified type.
        /// </summary>
        /// <param name="type">The type for which a plan should be created.</param>
        /// <returns>The type's activation plan.</returns>
        public IPlan GetPlan(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");

            plannerLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                IPlan plan;
                return plans.TryGetValue(type, out plan) ? plan : CreateNewPlan(type);
            }
            finally
            {
                plannerLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        ///     Creates an empty plan for the specified type.
        /// </summary>
        /// <param name="type">The type for which a plan should be created.</param>
        /// <returns>The created plan.</returns>
        protected virtual IPlan CreateEmptyPlan(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");
            return new Plan(type);
        }

        /// <summary>
        ///     Creates a new plan for the specified type.
        ///     This method requires an active reader lock!
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The newly created plan.</returns>
        private IPlan CreateNewPlan(Type type)
        {
            var lockCooki = plannerLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                IPlan plan;
                if (plans.TryGetValue(type, out plan))
                {
                    return plan;
                }

                plan = CreateEmptyPlan(type);
                plans.Add(type, plan);
                Strategies.Map(s => s.Execute(plan));

                return plan;
            }
            finally
            {
                plannerLock.DowngradeFromWriterLock(ref lockCooki);
            }
        }
    }
}