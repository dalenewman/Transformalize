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
using Transformalize.Libs.FileHelpers.Engines;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.DataLink.Storage
{
    /// <summary>
    ///     This class implements the <see cref="DataStorage" /> for plain text files.
    /// </summary>
    public sealed class FileStorage : DataStorage
    {
        private readonly FileHelperEngine mEngine;
        private string mFileName;

        /// <summary>Create an instance of this class to work with the specified type.</summary>
        /// <param name="type">The record class.</param>
        /// <param name="fileName">The target filename.</param>
        public FileStorage(Type type, string fileName) : base(type)
        {
            if (type == null)
                throw new BadUsageException("You need to pass a not null Type to the FileStorage.");

            mEngine = new FileHelperEngine(type);
            mErrorManager = mEngine.ErrorManager;

            mFileName = fileName;
        }

        #region "  SelectRecords  "

        /// <summary>Must Return the records from the DataSource (DB, Excel, etc)</summary>
        /// <returns>The extracted records.</returns>
        public override object[] ExtractRecords()
        {
            return mEngine.ReadFile(mFileName);
        }

        #endregion

        /// <summary>The engine behind the FileStorage.</summary>
        public FileHelperEngine Engine
        {
            get { return mEngine; }
        }

        /// <summary>The target file name.</summary>
        public string FileName
        {
            get { return mFileName; }
            set { mFileName = value; }
        }

        #region "  InsertRecords  "

        /// <summary>Must Insert the records in a DataSource (DB, Excel, etc)</summary>
        /// <param name="records">The records to insert.</param>
        public override void InsertRecords(object[] records)
        {
            if (mFileName == null || mFileName.Length == 0)
                throw new BadUsageException("You need to set a not empty FileName to the FileDataLinlProvider.");

            mEngine.WriteFile(mFileName, records);
        }

        #endregion
    }
}