#region "  © Copyright 2005-07 to Marcos Meli - http://www.marcosmeli.com.ar" 

// Errors, suggestions, contributions, send a mail to: marcos@filehelpers.com.

#endregion

using System;
using System.ComponentModel;
using System.Data;
using Transformalize.Libs.FileHelpers.Core;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.ErrorHandling;
using Transformalize.Libs.FileHelpers.Helpers;
using Transformalize.Libs.FileHelpers.Progress;

namespace Transformalize.Libs.FileHelpers.DataLink.Storage
{
    /// <summary>Base class for all the Storage classes of the library or the custom Storage classes.</summary>
    public abstract class DataStorage
    {
        private readonly Type mRecordType;

        /// <summary>The Object responsable for manage the errors.</summary>
        protected ErrorManager mErrorManager = new ErrorManager();

        /// <summary>You method handler used to notify progress.</summary>
        protected ProgressChangeHandler mNotifyHandler = null;

        /// <summary>Indicates the way to notify the progress.</summary>
        protected ProgressMode mProgressMode = ProgressMode.DontNotify;

        internal RecordInfo mRecordInfo;

        /// <summary>Creates an instance of this class.</summary>
        protected DataStorage(Type recordClass)
        {
            mRecordType = recordClass;
            mRecordInfo = new RecordInfo(recordClass);
        }

        #region "  Values <-> Record Convertions "

        /// <summary>Returns a record created from an Array of values</summary>
        /// <param name="values">The values used to created the record.</param>
        /// <returns>The just created record.</returns>
        protected object ValuesToRecord(object[] values)
        {
            return mRecordInfo.ValuesToRecord(values);
        }

        /// <summary>Returns an array of value based on a record.</summary>
        /// <param name="record">The source record.</param>
        /// <returns>An array with the values of each field</returns>
        protected object[] RecordToValues(object record)
        {
            return mRecordInfo.RecordToValues(record);
        }

        #endregion

        /// <summary>Returns the class that represent the records in the file.</summary>
        public Type RecordType
        {
            get { return mRecordType; }
        }

        /// <summary>The Object responsable for manage the errors.</summary>
        public ErrorManager ErrorManager
        {
            get { return mErrorManager; }
        }

        /// <summary>The number of fields in the record class.</summary>
        protected int RecordFieldCount
        {
            get { return mRecordInfo.mFieldCount; }
        }

        /// <summary>For internal Use.</summary>
        /// <param name="handler"></param>
        /// <param name="mode"></param>
        /// <param name="current"></param>
        /// <param name="total"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void Notify(ProgressChangeHandler handler, ProgressMode mode, int current, int total)
        {
            ProgressHelper.Notify(handler, mode, current, total);
        }

        /// <summary>Set the handler to the engine used to notify progress into the operations.</summary>
        /// <param name="handler">
        ///     The <see cref="ProgressChangeHandler" />
        /// </param>
        public void SetProgressHandler(ProgressChangeHandler handler)
        {
            SetProgressHandler(handler, ProgressMode.NotifyRecords);
        }

        /// <summary>Set the handler to the engine used to notify progress into the operations.</summary>
        /// <param name="handler">
        ///     Your <see cref="ProgressChangeHandler" /> method.
        /// </param>
        /// <param name="mode">
        ///     The <see cref="ProgressMode" /> to use.
        /// </param>
        public void SetProgressHandler(ProgressChangeHandler handler, ProgressMode mode)
        {
            mNotifyHandler = handler;

            if (mode == ProgressMode.NotifyBytes)
                throw new NotImplementedException("Not implemented yet. Planed for version 1.5.0");

            mProgressMode = mode;
        }

        /// <summary>Must Return the records from the DataSource (DB, Excel, etc)</summary>
        /// <returns>The extracted records.</returns>
        public abstract object[] ExtractRecords();

        /// <summary>Must Return the records from the DataSource (DB, Excel, etc)</summary>
        /// <returns>The extracted records.</returns>
        public DataTable ExtractRecordsAsDT()
        {
            var ri = new RecordInfo(RecordType);
            return ri.RecordsToDataTable(ExtractRecords());
        }

        /// <summary>Must Insert the records in a DataSource (DB, Excel, etc)</summary>
        /// <param name="records">The records to insert.</param>
        public abstract void InsertRecords(object[] records);


        /// <summary>Add an error to the ErrorCollection.</summary>
        /// <param name="lineNumber">The line when the error occurs.</param>
        /// <param name="ex">The exception throwed, can be null.</param>
        /// <param name="recordLine">The record values</param>
        protected void AddError(int lineNumber, Exception ex, string recordLine)
        {
            var e = new ErrorInfo();
            e.mLineNumber = lineNumber;
//			e.mColumnNumber = colNum;
            e.mExceptionInfo = ex;
            e.mRecordString = recordLine;

            mErrorManager.AddError(e);
        }
    }
}