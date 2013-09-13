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

#undef GENERICS
//#define GENERICS
//#if NET_2_0

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;
using Transformalize.Libs.FileHelpers.Core;
using Transformalize.Libs.FileHelpers.ErrorHandling;
using Transformalize.Libs.FileHelpers.Helpers;

#if GENERICS
using System.Collections.Generic;
#endif

namespace Transformalize.Libs.FileHelpers.Mapping
{
    /// <summary>
    ///     <para>A class to provide Record-DataTable operations.</para>
    ///     <para>(BETA QUALITY, use it at your own risk :P, API can change in future version)</para>
    /// </summary>
#if ! GENERICS
    public sealed class DataMapper
#else
    /// <typeparam name="T">The record Type</typeparam>
    public sealed class DataMapper<T>
#endif
    {
        private readonly RecordInfo mRecordInfo;

#if ! GENERICS
        /// <summary>
        ///     Create a new Mapping for the record Type 't'.
        /// </summary>
        /// <param name="recordType">The record class.</param>
        public DataMapper(Type recordType)
        {
            mRecordInfo = new RecordInfo(recordType);
        }

#else
    /// <summary>
    /// Create a new Mapping for the record Type 't'.
    /// </summary>
    /// <param name="t">The record class.</param>
        public DataMapper()
		{
			mRecordInfo = new RecordInfo(typeof(T));
		}

#endif
        private int mInitialColumnOffset;

        /// <summary>
        ///     Indicates the number of columns to discard in the result.
        /// </summary>
        public int InitialColumnOffset
        {
            get { return mInitialColumnOffset; }
            set { mInitialColumnOffset = value; }
        }

        /// <summary>
        ///     Add a new mapping between column at <paramref>columnIndex</paramref> and the fieldName with the specified <paramref>fieldName</paramref> name.
        /// </summary>
        /// <param name="columnIndex">The index in the Datatable</param>
        /// <param name="fieldName">The name of a fieldName in the Record Class</param>
        public void AddMapping(int columnIndex, string fieldName)
        {
            var map = new MappingInfo(mRecordInfo.mRecordType, fieldName);
            map.mDataColumnIndex = columnIndex + mInitialColumnOffset;
            mMappings.Add(map);
        }

        /// <summary>
        ///     Add a new mapping between column with <paramref>columnName</paramref> and the fieldName with the specified <paramref>fieldName</paramref> name.
        /// </summary>
        /// <param name="columnName">The name of the Column</param>
        /// <param name="fieldName">The name of a fieldName in the Record Class</param>
        public void AddMapping(string columnName, string fieldName)
        {
            var map = new MappingInfo(mRecordInfo.mRecordType, fieldName);
            map.mDataColumnName = columnName;
            mMappings.Add(map);
        }

        private readonly ArrayList mMappings = new ArrayList();

        /// <summary>
        ///     For each row in the datatable create a record.
        /// </summary>
        /// <param name="dt">The source Datatable</param>
        /// <returns>The mapped records contained in the DataTable</returns>
#if ! GENERICS
        public object[] MapDataTable2Records(DataTable dt)
        {
            var arr = new ArrayList(dt.Rows.Count);
#else
		public T[] MapDataTable2Records(DataTable dt)
        {
			List<T> arr = new List<T>(dt.Rows.Count);
#endif

            mMappings.TrimToSize();
            foreach (DataRow row in dt.Rows)
            {
                arr.Add(MapRow2Record(row));
            }

#if ! GENERICS
            return (object[]) arr.ToArray(mRecordInfo.mRecordType);
#else
			return arr.ToArray();
#endif
        }


        /// <summary>
        ///     Map a source row to a record.
        /// </summary>
        /// <param name="dr">The source DataRow</param>
        /// <returns>The mapped record containing the values of the DataRow</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
#if ! GENERICS
        public object MapRow2Record(DataRow dr)
        {
            var record = mRecordInfo.CreateRecordObject();
#else
		public T MapRow2Record(DataRow dr)
        {
			T record = (T) mRecordInfo.CreateRecordObject();
#endif

            for (var i = 0; i < mMappings.Count; i++)
            {
                ((MappingInfo) mMappings[i]).DataToField(dr, record);
            }

            return record;
        }

        /// <summary>
        ///     Map a source row to a record.
        /// </summary>
        /// <param name="dr">The already opened DataReader</param>
        /// <returns>The mapped record containing the values of the DataReader</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
#if ! GENERICS
        public object MapRow2Record(IDataReader dr)
        {
            var record = mRecordInfo.CreateRecordObject();
#else
		public T MapRow2Record(IDataReader dr)
		{
            T record = (T) mRecordInfo.CreateRecordObject();
#endif
            //TypedReference t = TypedReference.MakeTypedReference(record, new FieldInfo[]) null);

            for (var i = 0; i < mMappings.Count; i++)
            {
                ((MappingInfo) mMappings[i]).DataToField(dr, record);
            }

            return record;
        }

        /// <summary>
        ///     Create an automatic mapping for each column in the dt and each record field
        ///     (the mapping is made by Index)
        /// </summary>
        /// <param name="dt">The source Datatable</param>
        /// <returns>The mapped records contained in the DataTable</returns>
#if ! GENERICS
        public object[] AutoMapDataTable2RecordsByIndex(DataTable dt)
        {
            var arr = new ArrayList(dt.Rows.Count);
#else
        public T[] AutoMapDataTable2RecordsByIndex(DataTable dt)
		{
            List<T> arr = new List<T>(dt.Rows.Count);
#endif

            var fields =
                mRecordInfo.mRecordType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField |
                                                  BindingFlags.Instance | BindingFlags.IgnoreCase);


            if (fields.Length > dt.Columns.Count)
                throw new FileHelpersException("The data table has less fields than fields in the Type: " +
                                               mRecordInfo.mRecordType.Name);

            for (var i = 0; i < fields.Length; i++)
            {
                var map = new MappingInfo(fields[i]);
                map.mDataColumnIndex = i;
                mMappings.Add(map);
            }


            foreach (DataRow row in dt.Rows)
            {
#if ! GENERICS
                var record = mRecordInfo.CreateRecordObject();
#else
				T record = (T) mRecordInfo.CreateRecordObject();
#endif
                //TypedReference t = TypedReference.MakeTypedReference(record, new FieldInfo[]) null);

                for (var i = 0; i < mMappings.Count; i++)
                {
                    ((MappingInfo) mMappings[i]).DataToField(row, record);
                }

                arr.Add(record);
            }

#if ! GENERICS
            return (object[]) arr.ToArray(mRecordInfo.mRecordType);
#else
            return arr.ToArray();
#endif
        }


        /// <summary>
        ///     For each row in the datatable create a record.
        /// </summary>
        /// <param name="connection">A valid connection (Opened or not)</param>
        /// <param name="selectSql">The Sql statement used to return the records.</param>
        /// <returns>The mapped records contained in the DataTable</returns>
#if ! GENERICS
        public object[] MapDataReader2Records(IDbConnection connection, string selectSql)
        {
            object[] res;
#else
        public T[] MapDataReader2Records(IDbConnection connection, string selectSql)
        {
            T[] res;
#endif

            ExHelper.CheckNullParam(connection, "connection");
            ExHelper.CheckNullOrEmpty(selectSql, "selectSql");

            var cmd = connection.CreateCommand();
            cmd.CommandText = selectSql;

            IDataReader dr = null;
            var connectionOpened = false;

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    connectionOpened = true;
                }

                dr = cmd.ExecuteReader(CommandBehavior.SingleResult);
                res = MapDataReader2Records(dr);
            }
            finally
            {
                if (dr != null)
                    dr.Close();

                if (connectionOpened)
                    connection.Close();
            }

            return res;
        }

        /// <summary>
        ///     For each row in the datatable create a record.
        /// </summary>
        /// <param name="dr">The source DataReader</param>
        /// <returns>The mapped records contained in the DataTable</returns>
#if ! GENERICS
        public object[] MapDataReader2Records(IDataReader dr)
        {
            var arr = new ArrayList();
#else
		public T[] MapDataReader2Records(IDataReader dr)
        {
            List<T> arr = new List<T>();
#endif

            ExHelper.CheckNullParam(dr, "dr");

            mMappings.TrimToSize();


            var hasRows = true;
            if (dr is SqlDataReader)
                hasRows = ((SqlDataReader) dr).HasRows;
            else if (dr is OleDbDataReader)
                hasRows = ((OleDbDataReader) dr).HasRows;


            if (hasRows)
            {
                while (dr.Read())
                {
                    arr.Add(MapRow2Record(dr));
                }
            }
#if ! GENERICS
            return (object[]) arr.ToArray(mRecordInfo.mRecordType);
#else
			return arr.ToArray();
#endif
        }
    }
}

//#endif