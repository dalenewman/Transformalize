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

using System.Data;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.ConventionOperations
{
    /// <summary>
    ///     A convention based version of <see cref="OutputCommandOperation" />. Will
    ///     figure out as many things as it can on its own.
    /// </summary>
    public class ConventionOutputCommandOperation : OutputCommandOperation
    {
        public ConventionOutputCommandOperation(AbstractConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     Gets or sets the command to execute against the database
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        ///     Prepares the row by executing custom logic before passing on to the <see cref="PrepareCommand" />
        ///     for further process.
        /// </summary>
        /// <param name="row">The row.</param>
        protected virtual void PrepareRow(Row row)
        {
        }

        /// <summary>
        ///     Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="row">The row.</param>
        protected override void PrepareCommand(IDbCommand cmd, Row row)
        {
            PrepareRow(row);
            cmd.CommandText = Command;
            CopyRowValuesToCommandParameters(currentCommand, row);
        }
    }
}