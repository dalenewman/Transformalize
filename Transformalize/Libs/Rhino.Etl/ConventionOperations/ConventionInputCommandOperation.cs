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
    ///     A convention based version of <see cref="InputCommandOperation" />. Will
    ///     figure out as many things as it can on its own.
    /// </summary>
    public class ConventionInputCommandOperation : InputCommandOperation
    {
        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        public ConventionInputCommandOperation(AbstractConnection connection) : base(connection)
        {
            UseTransaction = false;
            Timeout = 0;
        }

        /// <summary>
        ///     Gets or sets the command to get the input from the database
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        ///     Gets or sets the timeout value for the database command
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        ///     Creates a row from the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected override Row CreateRowFromReader(IDataReader reader)
        {
            return Row.FromReader(reader);
        }

        /// <summary>
        ///     Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandText = Command;
            cmd.CommandTimeout = Timeout;
        }
    }
}