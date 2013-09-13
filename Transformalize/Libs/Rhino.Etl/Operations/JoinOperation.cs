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
using Transformalize.Libs.Rhino.Etl.Enumerables;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Perform a join between two sources. The left part of the join is optional and if not specified it will use the current pipeline as input.
    /// </summary>
    public abstract class JoinOperation : AbstractOperation
    {
        private readonly PartialProcessOperation left = new PartialProcessOperation();
        private readonly PartialProcessOperation right = new PartialProcessOperation();
        private readonly Dictionary<ObjectArrayKeys, List<Row>> rightRowsByJoinKey = new Dictionary<ObjectArrayKeys, List<Row>>();
        private readonly Dictionary<Row, object> rightRowsWereMatched = new Dictionary<Row, object>();
        private JoinType jointype;
        private string[] leftColumns;
        private bool leftRegistered;
        private string[] rightColumns;

        /// <summary>
        ///     Create an inner join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder InnerJoin
        {
            get { return new JoinBuilder(this, JoinType.Inner); }
        }

        /// <summary>
        ///     Create a left outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder LeftJoin
        {
            get { return new JoinBuilder(this, JoinType.Left); }
        }


        /// <summary>
        ///     Create a right outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder RightJoin
        {
            get { return new JoinBuilder(this, JoinType.Right); }
        }


        /// <summary>
        ///     Create a full outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder FullOuterJoin
        {
            get { return new JoinBuilder(this, JoinType.Full); }
        }

        /// <summary>
        ///     Sets the right part of the join
        /// </summary>
        /// <value>The right.</value>
        public JoinOperation Right(IOperation value)
        {
            right.Register(value);
            return this;
        }


        /// <summary>
        ///     Sets the left part of the join
        /// </summary>
        /// <value>The left.</value>
        public JoinOperation Left(IOperation value)
        {
            left.Register(value);
            leftRegistered = true;
            return this;
        }

        /// <summary>
        ///     Executes this operation
        /// </summary>
        /// <param name="rows">Rows in pipeline. These are only used if a left part of the join was not specified.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            PrepareForJoin();

            var rightEnumerable = GetRightEnumerable();

            var execute = left.Execute(leftRegistered ? null : rows);
            foreach (Row leftRow in new EventRaisingEnumerator(left, execute))
            {
                var key = leftRow.CreateKey(leftColumns);
                List<Row> rightRows;
                if (rightRowsByJoinKey.TryGetValue(key, out rightRows))
                {
                    foreach (var rightRow in rightRows)
                    {
                        rightRowsWereMatched[rightRow] = null;
                        yield return MergeRows(leftRow, rightRow);
                    }
                }
                else if ((jointype & JoinType.Left) != 0)
                {
                    var emptyRow = new Row();
                    yield return MergeRows(leftRow, emptyRow);
                }
                else
                {
                    LeftOrphanRow(leftRow);
                }
            }
            foreach (var rightRow in rightEnumerable)
            {
                if (rightRowsWereMatched.ContainsKey(rightRow))
                    continue;
                var emptyRow = new Row();
                if ((jointype & JoinType.Right) != 0)
                    yield return MergeRows(emptyRow, rightRow);
                else
                    RightOrphanRow(rightRow);
            }
        }

        private void PrepareForJoin()
        {
            Initialize();

            Guard.Against(left == null, "Left branch of a join cannot be null");
            Guard.Against(right == null, "Right branch of a join cannot be null");

            SetupJoinConditions();

            Guard.Against(leftColumns == null, "You must setup the left columns");
            Guard.Against(rightColumns == null, "You must setup the right columns");
        }

        private IEnumerable<Row> GetRightEnumerable()
        {
            IEnumerable<Row> rightEnumerable = new CachingEnumerable<Row>(
                new EventRaisingEnumerator(right, right.Execute(null))
                );
            foreach (var row in rightEnumerable)
            {
                var key = row.CreateKey(rightColumns);
                List<Row> rowsForKey;
                if (rightRowsByJoinKey.TryGetValue(key, out rowsForKey) == false)
                {
                    rightRowsByJoinKey[key] = rowsForKey = new List<Row>();
                }
                rowsForKey.Add(row);
            }
            return rightEnumerable;
        }

        /// <summary>
        ///     Called when a row on the right side was filtered by
        ///     the join condition, allow a derived class to perform
        ///     logic associated to that, such as logging
        /// </summary>
        protected virtual void RightOrphanRow(Row row)
        {
        }

        /// <summary>
        ///     Called when a row on the left side was filtered by
        ///     the join condition, allow a derived class to perform
        ///     logic associated to that, such as logging
        /// </summary>
        /// <param name="row">The row.</param>
        protected virtual void LeftOrphanRow(Row row)
        {
        }

        /// <summary>
        ///     Merges the two rows into a single row
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract Row MergeRows(Row leftRow, Row rightRow);

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        ///     Setups the join conditions.
        /// </summary>
        protected abstract void SetupJoinConditions();


        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            left.Dispose();
            right.Dispose();
        }


        /// <summary>
        ///     Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            left.PrepareForExecution(pipelineExecuter);
            right.PrepareForExecution(pipelineExecuter);
        }

        /// <summary>
        ///     Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Exception> GetAllErrors()
        {
            foreach (var error in left.GetAllErrors())
            {
                yield return error;
            }
            foreach (var error in right.GetAllErrors())
            {
                yield return error;
            }
        }

        /// <summary>
        ///     Occurs when    a row is processed.
        /// </summary>
        public override event Action<IOperation, Row> OnRowProcessed
        {
            add
            {
                foreach (IOperation    operation in new[] {left, right})
                    operation.OnRowProcessed += value;
                base.OnRowProcessed += value;
            }
            remove
            {
                foreach (IOperation    operation in new[] {left, right})
                    operation.OnRowProcessed -= value;
                base.OnRowProcessed -= value;
            }
        }

        /// <summary>
        ///     Occurs when    all    the    rows has finished processing.
        /// </summary>
        public override event Action<IOperation> OnFinishedProcessing
        {
            add
            {
                foreach (IOperation    operation in new[] {left, right})
                    operation.OnFinishedProcessing += value;
                base.OnFinishedProcessing += value;
            }
            remove
            {
                foreach (IOperation    operation in new[] {left, right})
                    operation.OnFinishedProcessing -= value;
                base.OnFinishedProcessing -= value;
            }
        }

        /// <summary>
        ///     Fluent interface to create joins
        /// </summary>
        public class JoinBuilder
        {
            private readonly JoinOperation parent;

            /// <summary>
            ///     Initializes a new instance of the <see cref="JoinBuilder" /> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="joinType">Type of the join.</param>
            public JoinBuilder(JoinOperation parent, JoinType joinType)
            {
                this.parent = parent;
                parent.jointype = joinType;
            }

            /// <summary>
            ///     Setup the left side of the join
            /// </summary>
            /// <param name="columns">The columns.</param>
            /// <returns></returns>
            public JoinBuilder Left(params string[] columns)
            {
                parent.leftColumns = columns;
                return this;
            }

            /// <summary>
            ///     Setup the right side of the join
            /// </summary>
            /// <param name="columns">The columns.</param>
            /// <returns></returns>
            public JoinBuilder Right(params string[] columns)
            {
                parent.rightColumns = columns;
                return this;
            }
        }
    }
}