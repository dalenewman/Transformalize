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

namespace FileHelpers.DataLink
{
#if NET_2_0

    /// <summary>This is a base class that implements the storage for <b>any</b> DB with ADO.NET support.</summary>
    /// <typeparam name="ConnectionClass">The ADO.NET connection class</typeparam>
    public sealed class GenericDatabaseStorage<ConnectionClass> : DatabaseStorage
        where ConnectionClass : IDbConnection, new()
    {
        #region Constructors
        /// <summary>Creates an object that implements the storage for <b>any</b> DB with ADO.NET support.</summary>        
        /// <param name="recordType">The record type to use.</param>
        /// <param name="connectionString">The connection string to </param>
        public GenericDatabaseStorage( Type recordType, string connectionString ) : base( recordType )
        {
            ConnectionString = connectionString;
        }

        #endregion

        #region Properties

        /// <summary></summary>
        protected override bool ExecuteInBatch
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Private Methods

        protected sealed override IDbConnection CreateConnection( )
        {
            if ( String.IsNullOrEmpty( ConnectionString ) )
                //throw new FileHelpersException( "The connection cannot open because connection string is null or empty." );
                throw new Exception( "The connection cannot open because connection string is null or empty." );

            ConnectionClass connection = new ConnectionClass();
            connection.ConnectionString = ConnectionString;

            return connection;
        }

        #endregion
    }
#endif
}