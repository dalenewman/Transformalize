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