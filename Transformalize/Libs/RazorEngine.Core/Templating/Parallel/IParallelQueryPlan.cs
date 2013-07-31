using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Libs.RazorEngine.Core.Templating.Parallel
{
    /// <summary>
    /// Defines the required contract for implementing a parallel query plan.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface IParallelQueryPlan<T>
    {
        #region Methods
        /// <summary>
        /// Creates a parallel query for the specified source.
        /// </summary>
        /// <param name="source">The source enumerable.</param>
        /// <returns>The parallel query.</returns>
        ParallelQuery<T> CreateQuery(IEnumerable<T> source);
        #endregion
    }
}