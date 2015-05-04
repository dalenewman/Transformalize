#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl.Enumerables;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;

namespace Transformalize.Libs.Rhino.Etl.Pipelines {
    /// <summary>
    ///     Execute all the actions syncronously without caching
    /// </summary>
    public class SingleThreadedNonCachedPipelineExecuter : AbstractPipelineExecuter {
        public SingleThreadedNonCachedPipelineExecuter(){
        }

        /// <summary>
        ///     Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected override IEnumerable<Row> DecorateEnumerableForExecution(IOperation operation, IEnumerable<Row> enumerator) {
            foreach (Row row in new EventRaisingEnumerator(operation, enumerator)) {
                yield return row;
            }
        }
    }
}