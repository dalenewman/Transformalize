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
using Transformalize.Libs.FileHelpers.Helpers;

namespace Transformalize.Libs.FileHelpers.DataLink.Storage
{
    /// <summary>
    ///     This is a base class that implements the <see cref="DataStorage" /> for Microsoft Access Files.
    /// </summary>
    public sealed class AccessStorage : DatabaseStorage
    {
        #region "  Constructors  "

        /// <summary>Creates a new AccessStorage.</summary>
        /// <param name="recordType">The Type of the Records</param>
        public AccessStorage(Type recordType)
            : this(recordType, string.Empty)
        {
        }

        /// <summary>Creates a new AccessStorage using the indicated file.</summary>
        /// <param name="recordType">The Type of the Records</param>
        /// <param name="accessFile">The MS Access file.</param>
        public AccessStorage(Type recordType, string accessFile)
            : base(recordType)
        {
            AccessFileName = accessFile;
            ConnectionString = DataBaseHelper.GetAccessConnection(AccessFileName, AccessFilePassword);
        }

        #endregion

        #region "  Create Connection and Command  "

        /// <summary>Must create an abstract connection object.</summary>
        /// <returns>An Abstract Connection Object.</returns>
        protected override sealed IDbConnection CreateConnection()
        {
            if (mAccessFile == null || mAccessFile == string.Empty)
                throw new BadUsageException("The AccessFileName can´t be null or empty.");

            return new OleDbConnection(ConnectionString);
        }

        #endregion

        #region "  AccessFileName  "

        private string mAccessFile = string.Empty;

        /// <summary>The file full path of the Microsoft Access File.</summary>
        public string AccessFileName
        {
            get { return mAccessFile; }
            set
            {
                mAccessFile = value;
                ConnectionString = DataBaseHelper.GetAccessConnection(AccessFileName, AccessFilePassword);
                ConnectionString = DataBaseHelper.GetAccessConnection(AccessFileName, AccessFilePassword);
            }
        }

        #endregion

        #region "  AccessFilePassword  "

        private string mAccessPassword = string.Empty;

        /// <summary>The password to the access database.</summary>
        public string AccessFilePassword
        {
            get { return mAccessPassword; }
            set
            {
                mAccessPassword = value;
                ConnectionString = DataBaseHelper.GetAccessConnection(AccessFileName, AccessFilePassword);
                ConnectionString = DataBaseHelper.GetAccessConnection(AccessFileName, AccessFilePassword);
            }
        }

        #endregion
    }
}

#endif