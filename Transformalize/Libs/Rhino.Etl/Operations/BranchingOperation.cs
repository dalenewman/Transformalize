using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl.Enumerables;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Branch the current pipeline flow into all its inputs
    /// </summary>
    public class BranchingOperation : AbstractBranchingOperation
    {
        /// <summary>
        ///     Executes this operation, sending the input of this operation
        ///     to all its child operations
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            var copiedRows = new CachingEnumerable<Row>(rows);

            foreach (IOperation operation in Operations)
            {
                IEnumerable<Row> cloned = copiedRows.Select(r => r.Clone());

                IEnumerable<Row> enumerable = operation.Execute(cloned);

                if (enumerable == null)
                    continue;

                IEnumerator<Row> enumerator = enumerable.GetEnumerator();
#pragma warning disable 642
                while (enumerator.MoveNext()) ;
#pragma warning restore 642
            }
            yield break;
        }
    }
}