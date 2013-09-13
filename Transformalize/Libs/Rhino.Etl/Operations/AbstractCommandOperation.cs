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
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Base class for operations that directly manipulate ADO.Net
    ///     It is important to remember that this is supposed to be a deep base class, not to be
    ///     directly inherited or used
    /// </summary>
    public abstract class AbstractCommandOperation : AbstractDatabaseOperation
    {
        /// <summary>
        ///     The current command
        /// </summary>
        protected IDbCommand currentCommand;

        protected AbstractCommandOperation(AbstractConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        ///     Adds the parameter to the current command
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected void AddParameter(string name, object value)
        {
            AddParameter(currentCommand, name, value);
        }

        /// <summary>
        ///     Begins a transaction conditionally based on the UseTransaction property
        /// </summary>
        /// <param name="connection">The IDbConnection object you are working with</param>
        /// <returns>An open IDbTransaction object or null.</returns>
        protected IDbTransaction BeginTransaction(IDbConnection connection)
        {
            return UseTransaction ? connection.BeginTransaction() : null;
        }
    }
}