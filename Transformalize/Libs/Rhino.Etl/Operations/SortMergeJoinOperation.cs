#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl.Enumerables;
using Transformalize.Main;

namespace Transformalize.Libs.Rhino.Etl.Operations {
    /// <summary>
    /// Perform a join between two sources. The left part of the join is optional and if not specified it will use the current pipeline as input.
    /// </summary>
    public abstract class SortMergeJoinOperation : AbstractOperation {
        private readonly PartialProcessOperation left;
        private readonly PartialProcessOperation right;
        private bool leftRegistered;

        protected SortMergeJoinOperation(ref Process process) {
            left = new PartialProcessOperation(ref process);
            right = new PartialProcessOperation(ref process);
        }

        /// <summary>
        /// The type of join to be performed
        /// </summary>
        protected abstract JoinType JoinType { get; }

        /// <summary>
        /// Sets the right part of the join
        /// </summary>
        /// <value>The right.</value>
        public SortMergeJoinOperation Right(IOperation value) {
            right.Register(value);
            return this;
        }

        /// <summary>
        /// Sets the left part of the join
        /// </summary>
        /// <value>The left.</value>
        public SortMergeJoinOperation Left(IOperation value) {
            left.Register(value);
            leftRegistered = true;
            return this;
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">Rows in pipeline. These are only used if a left part of the join was not specified.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            Initialize();

            Guard.Against(left == null, "Left branch of a join cannot be null");
            Guard.Against(right == null, "Right branch of a join cannot be null");

            var leftRows = new EventRaisingEnumerator(left, left.Execute(leftRegistered ? null : rows)).GetEnumerator();
            leftRows.MoveNext();
            var leftRow = (Row)leftRows.Current;

            var rightRows = new EventRaisingEnumerator(right, right.Execute(null)).GetEnumerator();
            rightRows.MoveNext();
            var rightRow = (Row)rightRows.Current;

            while (leftRow != null && rightRow != null) {
                var match = MatchJoinCondition(leftRow, rightRow);
                Row mergedRow = null;

                if (match == 0) {
                    mergedRow = MergeRows(leftRow, rightRow);
                    leftRow = leftRows.MoveNext()
                        ? (Row)leftRows.Current
                        : null;
                    rightRow = rightRows.MoveNext()
                        ? (Row)rightRows.Current
                        : null;
                } else if (match < 0) {
                    if ((JoinType & JoinType.Left) != 0)
                        mergedRow = MergeRows(leftRow, new Row());
                    else
                        LeftOrphanRow(leftRow);

                    leftRow = leftRows.MoveNext()
                        ? (Row)leftRows.Current
                        : null;
                } else if (match > 0) {
                    if ((JoinType & JoinType.Right) != 0)
                        mergedRow = MergeRows(new Row(), rightRow);
                    else
                        RightOrphanRow(rightRow);

                    rightRow = rightRows.MoveNext()
                        ? (Row)rightRows.Current
                        : null;
                }

                if (mergedRow != null)
                    yield return mergedRow;
            }

            if (leftRow == null && rightRow != null && (JoinType & JoinType.Right) != 0)
                yield return MergeRows(new Row(), rightRow);
        }

        /// <summary>
        /// Called when a row on the right side was filtered by
        /// the join condition, allow a derived class to perform 
        /// logic associated to that, such as logging
        /// </summary>
        protected virtual void RightOrphanRow(Row row) {

        }

        /// <summary>
        /// Called when a row on the left side was filtered by
        /// the join condition, allow a derived class to perform 
        /// logic associated to that, such as logging
        /// </summary>
        /// <param name="row">The row.</param>
        protected virtual void LeftOrphanRow(Row row) {

        }

        /// <summary>
        /// Merges the two rows into a single row
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract Row MergeRows(Row leftRow, Row rightRow);

        /// <summary>
        /// Check if the two rows match to the join condition.
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns>
        /// -1 if leftRow less than rightRow
        ///  0 if leftRow equals rightRow
        ///  1 if leftRow greater than rightRow
        /// </returns>
        protected abstract int MatchJoinCondition(Row leftRow, Row rightRow);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize() {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose() {
            left.Dispose();
            right.Dispose();
        }

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter) {
            left.PrepareForExecution(pipelineExecuter);
            right.PrepareForExecution(pipelineExecuter);
        }

        /// <summary>
        /// Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Exception> GetAllErrors() {
            foreach (var error in left.GetAllErrors()) {
                yield return error;
            }
            foreach (var error in right.GetAllErrors()) {
                yield return error;
            }
        }
    }
}