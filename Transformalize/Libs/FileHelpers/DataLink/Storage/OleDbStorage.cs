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

#if ! MINI

using System;
using System.Data;
using System.Data.OleDb;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.DataLink.Storage
{
    /// <summary>
    ///     This is a base class that implements the <see cref="DataStorage" /> for Microsoft Access Files.
    /// </summary>
    public sealed class OleDbStorage : DatabaseStorage
    {
        #region "  Constructors  "

        /// <summary>
        ///     Create a new OleDbStorage based in the record type and in the connection string.
        /// </summary>
        /// <param name="recordType">The Type of the records.</param>
        /// <param name="oleDbConnString">The conection string used to create the OleDbConnection.</param>
        public OleDbStorage(Type recordType, string oleDbConnString) : base(recordType)
        {
            ConnectionString = oleDbConnString;
        }

        #endregion

        #region "  Create Connection and Command  "

        /// <summary>Must create an abstract connection object.</summary>
        /// <returns>An Abstract Connection Object.</returns>
        protected override sealed IDbConnection CreateConnection()
        {
            if (ConnectionString == null || ConnectionString == string.Empty)
                throw new BadUsageException("The OleDb Connection string can´t be null or empty.");
            return new OleDbConnection(ConnectionString);
        }

        #endregion
    }
}

#endif