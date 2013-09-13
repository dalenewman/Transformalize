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
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.DataLink.Storage
{
    /// <summary>
    ///     <para>
    ///         This class implements the <see cref="DataStorage" /> for Microsoft Excel Files using a OleDbConnection
    ///     </para>
    /// </summary>
    public sealed class ExcelStorageOleDb : DataStorage
    {
        private static string CreateConnectionString(string file, bool hasHeaders)
        {
            var mConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source={0};Extended Properties=\"Excel 8.0;{1}\"";
            var extProps = string.Empty;

            if (hasHeaders)
                extProps += " HDR=YES;";
            else
                extProps += " HDR=NO;";


            return String.Format(mConnectionString, file, extProps);
        }

        /// <summary>
        ///     An useful method to direct extract a DataTable from an Excel File without need to instanciate anything.
        /// </summary>
        /// <param name="file">The Excel file to read.</param>
        /// <param name="row">The initial row (the first is 1)</param>
        /// <param name="col">The initial column (the first is 1)</param>
        /// <param name="hasHeader">Indicates is there ir a header row.</param>
        /// <returns>The DataTable generated reading the excel file at the specified position.</returns>
        public static DataTable ExtractDataTable(string file, int row, int col, bool hasHeader)
        {
            OleDbConnection connExcel;
            //private OleDbDataAdapter mDaExcel;

            connExcel = new OleDbConnection(CreateConnectionString(file, hasHeader));
            connExcel.Open();
            var res = new DataTable();

            var sheetName = GetFirstSheet(connExcel);

            var sheet = sheetName + (sheetName.EndsWith("$") ? "" : "$");
            var command = String.Format("SELECT * FROM [{0}]", sheet);

            var cm = new OleDbCommand(command, connExcel);
            var da = new OleDbDataAdapter(cm);
            da.Fill(res);

            connExcel.Close();
            return res;
        }

        #region "  Constructors  "

        /// <summary>Create a new ExcelStorage to work with the specified type</summary>
        /// <param name="recordType">The type of records.</param>
        public ExcelStorageOleDb(Type recordType) : base(recordType)
        {
        }

        /// <summary>Create a new ExcelStorage to work with the specified type</summary>
        /// <param name="recordType">The type of records.</param>
        /// <param name="startRow">The row of the first data cell. Begining in 1.</param>
        /// <param name="startCol">The column of the first data cell. Begining in 1.</param>
        public ExcelStorageOleDb(Type recordType, int startRow, int startCol) : this(recordType)
        {
            mStartColumn = startCol;
            mStartRow = startRow;
        }

        /// <summary>Create a new ExcelStorage to work with the specified type</summary>
        /// <param name="recordType">The type of records.</param>
        /// <param name="startRow">The row of the first data cell. Begining in 1.</param>
        /// <param name="startCol">The column of the first data cell. Begining in 1.</param>
        /// <param name="fileName">The file path to work with.</param>
        public ExcelStorageOleDb(Type recordType, string fileName, int startRow, int startCol)
            : this(recordType, startRow, startCol)
        {
            mFileName = fileName;
        }

        #endregion

        #region "  Private Fields  "

        private OleDbConnection mCnExcel;
        private OleDbDataAdapter mDaExcel;
        private DataSet mDsExcel;
        private DataTable mDtExcel;
        private string mFileName = String.Empty;
        private string mSheetName = String.Empty;
        private int mStartColumn = 1;
        private int mStartRow = 1;

        #endregion

        #region "  Public Properties  "

        private bool mOverrideFile = true;

        /// <summary>The Start Row where is the data. Starting at 1.</summary>
        public int StartRow
        {
            get { return mStartRow; }
            set { mStartRow = value; }
        }

        /// <summary>The Start Column where is the data. Starting at 1.</summary>
        public int StartColumn
        {
            get { return mStartColumn; }
            set { mStartColumn = value; }
        }

        /// <summary>
        ///     Indicates if in the StartColumn and StartRow has headers or not.
        /// </summary>
        public bool HasHeaderRow { get; set; }

        /// <summary>The Excel File Name.</summary>
        public string FileName
        {
            get { return mFileName; }
            set { mFileName = value; }
        }

        /// <summary>The Excel Sheet Name, if empty means the current worksheet in the file.</summary>
        public string SheetName
        {
            get { return mSheetName; }
            set { mSheetName = value; }
        }

        /// <summary>Indicates what the Storage does if the file exist.</summary>
        public bool OverrideFile
        {
            get { return mOverrideFile; }
            set { mOverrideFile = value; }
        }

        ///// <summary>Indicates the Connection String to Open the Excel File.</summary>
        //public string ConnectionString
        //{
        //    get { return mConnectionString; }
        //    set { mConnectionString = value; }
        //}

        #endregion

        #region "  OpenConnection  "

        private void OpenConnection()
        {
            try
            {
                if (mCnExcel != null && mCnExcel.State == ConnectionState.Open)
                {
                    mCnExcel.Close();
#if NET_2_0
                mCnExcel.Dispose();
#endif
                }

                mCnExcel = new OleDbConnection(CreateConnectionString(mFileName, HasHeaderRow));
                mCnExcel.Open();
                mDtExcel = new DataTable();
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "  CloseAndCleanUp  "

        private void CloseAndCleanUp()
        {
            if (mCnExcel != null && mCnExcel.State == ConnectionState.Open)
            {
                mCnExcel.Close();
                //mCnExcel.Dispose();
            }
        }

        #endregion

        #region "  OpenWorkbook  "

        private void OpenWorkbook()
        {
            var info = new FileInfo(mFileName);
            if (info.Exists == false)
                throw new FileNotFoundException("Excel File '" + mFileName + "' not found.", mFileName);


            OpenConnection();


            if (mSheetName == null || mSheetName == string.Empty)
            {
                string sheet;
                sheet = GetFirstSheet(mCnExcel);
                mSheetName = sheet;
            }

            try
            {
                var sheet = SheetName + (mSheetName.EndsWith("$") ? "" : "$");
                var command = String.Format("SELECT * FROM [{0}]", sheet);
                var cm = new OleDbCommand(command, mCnExcel);
                mDaExcel = new OleDbDataAdapter(cm);
                mDsExcel = new DataSet();
                mDaExcel.Fill(mDsExcel);
                mDtExcel = mDsExcel.Tables[0];
            }
            catch
            {
                throw new BadUsageException("The sheet '" + mSheetName + "' was not found in the workbook.");
            }
        }

        private static string GetFirstSheet(OleDbConnection conn)
        {
#if NET_2_0
			DataTable dt = conn.GetSchema("Tables");
#else
            var dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
#endif

            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["TABLE_NAME"].ToString();
            else
                throw new BadUsageException("A Valid sheet was not found in the workbook.");
        }

        #endregion

        #region "  CreateWorkbook methods  "

        private void OpenOrCreateWorkbook(string filename)
        {
            if (!File.Exists(filename))
                CreateWorkbook();

            OpenWorkbook();
        }

        private void CreateWorkbook()
        {
            if (mCnExcel != null && mCnExcel.State == ConnectionState.Open)
            {
                mCnExcel.Close();
#if NET_2_0
                mCnExcel.Dispose();
#endif
            }

            //Delete the file if Exists
            if (File.Exists(mFileName) && mOverrideFile)
            {
                File.Delete(mFileName);
            }

            OpenConnection();

            var cmCreate = "";
            if (mSheetName == null || mSheetName == string.Empty)
            {
                mSheetName = "Sheet1$";
            }


            var cm = new OleDbCommand();
            cm.Connection = mCnExcel;
            cmCreate = string.Format("CREATE TABLE [{0}] (", mSheetName.Replace("$", ""));
            foreach (var field in RecordType.GetFields())
            {
                cmCreate += field.Name + " char(255),";
            }
            if (cmCreate.EndsWith(","))
            {
                cmCreate = cmCreate.Substring(0, cmCreate.Length - 1);
            }
            cmCreate += ")";

            cm.CommandText = cmCreate;
            cm.ExecuteNonQuery();

            mCnExcel.Close();
        }

        #endregion

        #region "  SaveWorkbook  "

        /// <summary>
        ///     Update Rows in Workbook and Close the File
        /// </summary>
        private void SaveWorkbook()
        {
            #region Build and Add the Insert Command to DataAdapter

            var cmdIns = string.Format("INSERT INTO [{0}] (", mSheetName + (mSheetName.EndsWith("$") ? "" : "$"));
            var values = "";
            foreach (DataColumn col in mDtExcel.Columns)
            {
                cmdIns += col.ColumnName + ",";
                values += "?,";
            }
            cmdIns = cmdIns.Substring(0, cmdIns.Length - 1); //Clean the ","
            values = values.Substring(0, values.Length - 1); //Clean the ","
            cmdIns += ") VALUES( " + values + ")";

            var cmd = new OleDbCommand(cmdIns, mCnExcel);
            mDaExcel.InsertCommand = cmd;
            foreach (DataColumn col in mDtExcel.Columns)
            {
                cmd.Parameters.AddWithValue("@" + col.ColumnName, col.DataType).SourceColumn = col.ColumnName;
            }

            #endregion

            mDaExcel.Update(mDtExcel);

            CloseAndCleanUp();
        }

        #endregion

        #region "  RowValues  "

        private object[] RowValues(int row, int startCol, int numberOfCols)
        {
            if (mDtExcel == null)
            {
                return null;
            }
            object[] res;

            if (numberOfCols == 1)
            {
                if (startCol <= mDtExcel.Columns.Count)
                {
                    res = new[] {mDtExcel.Rows[row][startCol - 1]};
                }
                else
                {
                    throw new BadUsageException("The Start Column is Greater Than the Columns Count in " + mSheetName);
                }
            }
            else
            {
                res = new object[numberOfCols];
                var counter = 0;
                var colPos = startCol - 1;

                //If the numbers of selected columns is Greater Than columns in DataTable
                while (counter < res.Length && colPos < mDtExcel.Columns.Count)
                {
                    res[counter] = mDtExcel.Rows[row][colPos];
                    counter++;
                    colPos++;
                }
            }

            return res;
        }

        private void WriteRowValues(object[] values, int row, int startCol)
        {
            if (mDtExcel == null)
                return;

            DataRow dtRow;
            if (row > mDtExcel.Rows.Count - 1)
            {
                dtRow = mDtExcel.NewRow();
                mDtExcel.Rows.Add(dtRow);
            }
            else
            {
                dtRow = mDtExcel.Rows[row];
            }

            var colPos = startCol - 1;
            var i = 0;
            while (colPos < mDtExcel.Columns.Count && i < values.Length)
            {
                dtRow[colPos] = values[i];
                colPos++;
                i++;
            }
        }

        #endregion

        #region "  InsertRecords  "

        /// <summary>Insert all the records in the specified Excel File.</summary>
        /// <param name="records">The records to insert.</param>
        public override void InsertRecords(object[] records)
        {
            if (records == null || records.Length == 0)
                return;

            try
            {
                var recordNumber = 0;
                Notify(mNotifyHandler, mProgressMode, 0, records.Length);

                OpenOrCreateWorkbook(mFileName);

                mDtExcel = mDsExcel.Tables[0].Clone();

                //Verify Properties Limits.
                ValidatePropertiesForInsert(records);

                for (var row = mStartRow - 1; row < records.Length; row++)
                {
                    recordNumber++;
                    Notify(mNotifyHandler, mProgressMode, recordNumber, records.Length);

                    WriteRowValues(RecordToValues(records[row]), row, mStartColumn);
                }

                SaveWorkbook();
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseAndCleanUp();
            }
        }

        #endregion

        #region "  ExtractRecords  "

        /// <summary>Returns the records extracted from Excel file.</summary>
        /// <returns>The extracted records.</returns>
        public override object[] ExtractRecords()
        {
            var res = new ArrayList();

            try
            {
                Notify(mNotifyHandler, mProgressMode, 0, -1);

                var colValues = new object[mRecordInfo.mFieldCount];

                OpenWorkbook();

                //Verify Properties Limits.
                ValidatePropertiesForExtract();

                //mStartRow-1 because rows start at 0, and the user
                //can be assign StartRow =2 
                for (var recordNumber = mStartRow - 1; recordNumber < mDtExcel.Rows.Count; recordNumber++)
                {
                    try
                    {
                        Notify(mNotifyHandler, mProgressMode, recordNumber, -1);

                        colValues = RowValues(recordNumber, mStartColumn, mRecordInfo.mFieldCount);

                        var record = ValuesToRecord(colValues);
                        res.Add(record);
                    }
                    catch (Exception ex)
                    {
                        switch (mErrorManager.ErrorMode)
                        {
                            case ErrorMode.ThrowException:
                                throw;
                            case ErrorMode.IgnoreAndContinue:
                                break;
                            case ErrorMode.SaveAndContinue:
                                AddError(recordNumber + 1, ex, string.Empty);
                                break;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseAndCleanUp();
            }

            return (object[]) res.ToArray(RecordType);
        }

        #endregion

        #region Validation

        private void ValidatePropertiesForExtract()
        {
            if (mFileName == String.Empty)
                throw new BadUsageException("You need to specify the WorkBookFile of the ExcelDataLink.");

            if (mStartRow <= 0)
                throw new BadUsageException("The StartRow Property is Invalid. Must be Greater or Equal Than 1.");

            if (mStartRow > mDtExcel.Rows.Count)
                throw new BadUsageException("The StartRow Property is Invalid. Must be Less or Equal to Worksheet row's count.");

            if (mStartColumn <= 0)
                throw new BadUsageException("The StartColumn Property is Invalid. Must be Greater or Equal Than 1.");

            if (mStartColumn > mDtExcel.Columns.Count)
                throw new BadUsageException("The StartColumn Property is Invalid. Must be Less or Equal To Worksheet Column's count.");
        }

        private void ValidatePropertiesForInsert(object[] records)
        {
            if (mFileName == String.Empty)
                throw new BadUsageException("You need to specify the WorkBookFile of the ExcelDataLink.");

            if (mStartRow <= 0)
                throw new BadUsageException("The StartRow Property is Invalid. Must be Greater or Equal Than 1.");

            if (mStartRow > records.Length)
                throw new BadUsageException("The StartRow Property is Invalid. Must be Less or Equal to Worksheet row's count.");

            if (mStartColumn <= 0)
                throw new BadUsageException("The StartColumn Property is Invalid. Must be Greater or Equal Than 1.");

            if (mStartColumn > mRecordInfo.mFieldCount)
                throw new BadUsageException("The StartColumn Property is Invalid. Must be Less or Equal To Record Type Propertie's count.");
        }

        #endregion
    }
}