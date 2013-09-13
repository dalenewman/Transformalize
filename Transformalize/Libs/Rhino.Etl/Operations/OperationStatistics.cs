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
using System.Threading;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Contains the statistics for an operation
    /// </summary>
    public class OperationStatistics
    {
        private DateTime? end;
        private long outputtedRows;
        private DateTime? start;

        /// <summary>
        ///     Gets number of the outputted rows.
        /// </summary>
        /// <value>The processed rows.</value>
        public long OutputtedRows
        {
            get { return outputtedRows; }
        }

        /// <summary>
        ///     Gets the duration this operation has executed
        /// </summary>
        /// <value>The duration.</value>
        public TimeSpan Duration
        {
            get
            {
                if (start == null || end == null)
                    return new TimeSpan();

                return end.Value - start.Value;
            }
        }

        /// <summary>
        ///     Mark the start time
        /// </summary>
        public void MarkStarted()
        {
            start = DateTime.Now;
        }

        /// <summary>
        ///     Mark the end time
        /// </summary>
        public void MarkFinished()
        {
            end = DateTime.Now;
        }

        /// <summary>
        ///     Marks a processed row.
        /// </summary>
        public void MarkRowProcessed()
        {
            Interlocked.Increment(ref outputtedRows);
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return OutputtedRows + " Rows in " + Duration;
        }

        /// <summary>
        ///     Adds to the count of the output rows.
        /// </summary>
        /// <param name="rowProcessed">The row processed.</param>
        public void AddOutputRows(long rowProcessed)
        {
            Interlocked.Increment(ref outputtedRows);
        }
    }
}