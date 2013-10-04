#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Data;
using System.Data.OleDb;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.Excel
{
    public class ExcelReader : ExcelHelper
    {
        #region "  Constructors  "

        public ExcelReader()
        {
        }

        public ExcelReader(int startRow, int startCol) : base(startRow, startCol)
        {
        }

        #endregion

        private bool mReadAllAsText;

        public bool ReadAllAsText
        {
            get { return mReadAllAsText; }
            set { mReadAllAsText = value; }
        }

        private void ValidatePropertiesForExtract()
        {
            if (StartRow <= 0)
                throw new BadUsageException("The StartRow Property is Invalid. Must be Greater or Equal Than 1.");

//            if (this.StartRow > mDtExcel.Rows.Count)
//                throw new BadUsageException("The StartRow Property is Invalid. Must be Less or Equal to Worksheet row's count.");

            if (StartColumn <= 0)
                throw new BadUsageException("The StartColumn Property is Invalid. Must be Greater or Equal Than 1.");

//            if (this.StartColumn > mDtExcel.Columns.Count)
//                throw new BadUsageException("The StartColumn Property is Invalid. Must be Less or Equal To Worksheet Column's count.");
        }


        public DataTable ExtractDataTable(string file)
        {
            return ExtractDataTable(file, StartRow, StartColumn);
        }

        public DataTable ExtractDataTable(string file, int row, int col)
        {
            ValidatePropertiesForExtract();

            OleDbConnection connExcel;
            //private OleDbDataAdapter mDaExcel;

            connExcel = new OleDbConnection(CreateConnectionString(file));
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

        protected override string ExtraProps()
        {
            if (mReadAllAsText)
                return " IMEX=1;";

            return string.Empty;
        }
    }
}