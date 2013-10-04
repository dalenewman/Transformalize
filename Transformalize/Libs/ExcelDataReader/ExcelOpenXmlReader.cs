#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;
using Transformalize.Libs.ExcelDataReader.Core;
using Transformalize.Libs.ExcelDataReader.Core.OpenXmlFormat;

namespace Transformalize.Libs.ExcelDataReader
{
	public class ExcelOpenXmlReader : IExcelDataReader
	{
	    #region Members

	    private const string COLUMN = "Column";

	    private readonly List<int> _defaultDateTimeStyles;
	    private object[] _cellsValues;
	    private int _depth;
	    private int _emptyRowCount;
	    private string _exceptionMessage;
	    private bool _isClosed;
	    private bool _isFirstRead;
	    private bool _isFirstRowAsColumnNames;
	    private bool _isValid;
	    private int _resultIndex;
	    private object[] _savedCellsValues;
	    private Stream _sheetStream;
	    private XlsxWorkbook _workbook;
	    private XmlReader _xmlReader;
	    private ZipWorker _zipWorker;

	    private bool disposed;

	    #endregion

	    internal ExcelOpenXmlReader()
		{
			_isValid = true;
			_isFirstRead = true;

			_defaultDateTimeStyles = new List<int>(new[] 
			{
				14, 15, 16, 17, 18, 19, 20, 21, 22, 45, 46, 47
			});
		}

		private void ReadGlobals()
		{
			_workbook = new XlsxWorkbook(
				_zipWorker.GetWorkbookStream(),
				_zipWorker.GetWorkbookRelsStream(),
				_zipWorker.GetSharedStringsStream(),
				_zipWorker.GetStylesStream());

			CheckDateTimeNumFmts(_workbook.Styles.NumFmts);

		}

		private void CheckDateTimeNumFmts(List<XlsxNumFmt> list)
		{
			if (list.Count == 0) return;

			foreach (var numFmt in list)
			{
				if (string.IsNullOrEmpty(numFmt.FormatCode)) continue;
				var fc = numFmt.FormatCode;

				int pos;
				while ((pos = fc.IndexOf('"')) > 0)
				{
					var endPos = fc.IndexOf('"', pos + 1);

					if (endPos > 0) fc = fc.Remove(pos, endPos - pos + 1);
				}

				if (fc.IndexOfAny(new[] { 'y', 'm', 'd', 's', 'h' }) >= 0)
				{
					_defaultDateTimeStyles.Add(numFmt.Id);
				}
			}
		}

		private void ReadSheetGlobals(XlsxWorksheet sheet)
		{
			_sheetStream = _zipWorker.GetWorksheetStream(sheet.Path);

			if (null == _sheetStream) return;

			_xmlReader = XmlReader.Create(_sheetStream);

			while (_xmlReader.Read())
			{
				if (_xmlReader.NodeType == XmlNodeType.Element && _xmlReader.Name == XlsxWorksheet.N_dimension)
				{
					var dimValue = _xmlReader.GetAttribute(XlsxWorksheet.A_ref);

					if (dimValue.IndexOf(':') > 0)
					{
						sheet.Dimension = new XlsxDimension(dimValue);
					}
					else
					{
						_xmlReader.Close();
						_sheetStream.Close();
					}

					break;
				}
			}
		}

		private bool ReadSheetRow(XlsxWorksheet sheet)
		{
			if (null == _xmlReader) return false;

			if (_emptyRowCount != 0)
			{
				_cellsValues = new object[sheet.ColumnsCount];
				_emptyRowCount--;
				_depth++;

				return true;
			}

			if (_savedCellsValues != null)
			{
				_cellsValues = _savedCellsValues;
				_savedCellsValues = null;
				_depth++;

				return true;
			}

            if ((_xmlReader.NodeType == XmlNodeType.Element && _xmlReader.Name == XlsxWorksheet.N_row) ||
                _xmlReader.ReadToFollowing(XlsxWorksheet.N_row))
			{
				_cellsValues = new object[sheet.ColumnsCount];

				var rowIndex = int.Parse(_xmlReader.GetAttribute(XlsxWorksheet.A_r));
				if (rowIndex != (_depth + 1))
				{
					_emptyRowCount = rowIndex - _depth - 1;
				}
				var hasValue = false;
				var a_s = String.Empty;
				var a_t = String.Empty;
				var a_r = String.Empty;
				var col = 0;
				var row = 0;

 				while (_xmlReader.Read())
 				{
 					if (_xmlReader.Depth == 2) break;
 
					if (_xmlReader.NodeType == XmlNodeType.Element)
 					{
						hasValue = false;
 
						if (_xmlReader.Name == XlsxWorksheet.N_c)
						{
							a_s = _xmlReader.GetAttribute(XlsxWorksheet.A_s);
							a_t = _xmlReader.GetAttribute(XlsxWorksheet.A_t);
							a_r = _xmlReader.GetAttribute(XlsxWorksheet.A_r);
							XlsxDimension.XlsxDim(a_r, out col, out row);
						}
						else if (_xmlReader.Name == XlsxWorksheet.N_v)
						{
							hasValue = true;
						}
					}
 
					if (_xmlReader.NodeType == XmlNodeType.Text && hasValue)
					{
						object o = _xmlReader.Value;
 
 						if (null != a_t && a_t == XlsxWorksheet.A_s)
 						{
							o = _workbook.SST[Convert.ToInt32(o)];
						}
						else if (null != a_s)
						{
							var xf = _workbook.Styles.CellXfs[int.Parse(a_s)];

							if (xf.ApplyNumberFormat && IsDateTimeStyle(xf.NumFmtId) && o != null && o.ToString() != string.Empty)
							{
							    try {
                                    o = DateTime.FromOADate(Convert.ToDouble(o, CultureInfo.InvariantCulture));    
							    } catch {
							        // just don't try and convert it then
							    }
								
							}
						}

						if (col - 1 < _cellsValues.Length)
							_cellsValues[col - 1] = o;
					}
				}

				if (_emptyRowCount > 0)
				{
					_savedCellsValues = _cellsValues;
					return ReadSheetRow(sheet);
				}
				else
					_depth++;

				return true;
			}

			_xmlReader.Close();
			if (_sheetStream != null) _sheetStream.Close();

			return false;
		}

		private bool InitializeSheetRead()
		{
			if (ResultsCount <= 0) return false;

			ReadSheetGlobals(_workbook.Sheets[_resultIndex]);

			if (_workbook.Sheets[_resultIndex].Dimension == null) return false;

			_isFirstRead = false;

			_depth = 0;
			_emptyRowCount = 0;

			return true;
		}

		private bool IsDateTimeStyle(int styleId)
		{
			return _defaultDateTimeStyles.Contains(styleId);
		}

	    #region IExcelDataReader Members

	    public void Initialize(Stream fileStream)
	    {
	        _zipWorker = new ZipWorker();
	        _zipWorker.Extract(fileStream);

	        if (!_zipWorker.IsValid)
	        {
	            _isValid = false;
	            _exceptionMessage = _zipWorker.ExceptionMessage;

	            Close();

	            return;
	        }

	        ReadGlobals();
	    }

	    public DataSet AsDataSet()
	    {
	        return AsDataSet(true);
	    }

	    public DataSet AsDataSet(bool convertOADateTime)
	    {
	        if (!_isValid) return null;

	        var dataset = new DataSet();

	        for (_resultIndex = 0; _resultIndex < _workbook.Sheets.Count; _resultIndex++)
	        {
	            var table = new DataTable(_workbook.Sheets[_resultIndex].Name);

	            ReadSheetGlobals(_workbook.Sheets[_resultIndex]);

	            if (_workbook.Sheets[_resultIndex].Dimension == null) continue;

	            _depth = 0;
	            _emptyRowCount = 0;

	            //DataTable columns
	            if (!_isFirstRowAsColumnNames)
	            {
	                for (var i = 0; i < _workbook.Sheets[_resultIndex].ColumnsCount; i++)
	                {
	                    table.Columns.Add();
	                }
	            }
	            else if (ReadSheetRow(_workbook.Sheets[_resultIndex]))
	            {
	                for (var index = 0; index < _cellsValues.Length; index++)
	                {
	                    if (_cellsValues[index] != null && _cellsValues[index].ToString().Length > 0)
	                        table.Columns.Add(_cellsValues[index].ToString());
	                    else
	                        table.Columns.Add(string.Concat(COLUMN, index));
	                }
	            }
	            else continue;

	            while (ReadSheetRow(_workbook.Sheets[_resultIndex]))
	            {
	                table.Rows.Add(_cellsValues);
	            }

	            if (table.Rows.Count > 0)
	                dataset.Tables.Add(table);
	        }

	        return dataset;
	    }

	    public bool IsFirstRowAsColumnNames
	    {
	        get
	        {
	            return _isFirstRowAsColumnNames;
	        }
	        set
	        {
	            _isFirstRowAsColumnNames = value;
	        }
	    }

	    public bool IsValid
	    {
	        get { return _isValid; }
	    }

	    public string ExceptionMessage
	    {
	        get { return _exceptionMessage; }
	    }

	    public string Name
	    {
	        get
	        {
	            return (_resultIndex >= 0 && _resultIndex < ResultsCount) ? _workbook.Sheets[_resultIndex].Name : null;
	        }
	    }

	    public void Close()
	    {
	        _isClosed = true;

	        if (_xmlReader != null) _xmlReader.Close();

	        if (_sheetStream != null) _sheetStream.Close();

	        if (_zipWorker != null) _zipWorker.Dispose();
	    }

	    public int Depth
	    {
	        get { return _depth; }
	    }

	    public int ResultsCount
	    {
	        get { return _workbook == null ? -1 : _workbook.Sheets.Count; }
	    }

	    public bool IsClosed
	    {
	        get { return _isClosed; }
	    }

	    public bool NextResult()
	    {
	        if (_resultIndex >= (ResultsCount - 1)) return false;

	        _resultIndex++;

	        _isFirstRead = true;

	        return true;
	    }

	    public bool Read()
	    {
	        if (!_isValid) return false;

	        if (_isFirstRead && !InitializeSheetRead())
	        {
	            return false;
	        }

	        return ReadSheetRow(_workbook.Sheets[_resultIndex]);
	    }

	    public int FieldCount
	    {
	        get { return (_resultIndex >= 0 && _resultIndex < ResultsCount) ? _workbook.Sheets[_resultIndex].ColumnsCount : -1; }
	    }

	    public bool GetBoolean(int i)
	    {
	        if (IsDBNull(i)) return false;

	        return Boolean.Parse(_cellsValues[i].ToString());
	    }

	    public DateTime GetDateTime(int i)
	    {
	        if (IsDBNull(i)) return DateTime.MinValue;

	        try
	        {
	            return (DateTime)_cellsValues[i];
	        }
	        catch (InvalidCastException)
	        {
	            return DateTime.MinValue;
	        }

	    }

	    public decimal GetDecimal(int i)
	    {
	        if (IsDBNull(i)) return decimal.MinValue;

	        return decimal.Parse(_cellsValues[i].ToString());
	    }

	    public double GetDouble(int i)
	    {
	        if (IsDBNull(i)) return double.MinValue;

	        return double.Parse(_cellsValues[i].ToString());
	    }

	    public float GetFloat(int i)
	    {
	        if (IsDBNull(i)) return float.MinValue;

	        return float.Parse(_cellsValues[i].ToString());
	    }

	    public short GetInt16(int i)
	    {
	        if (IsDBNull(i)) return short.MinValue;

	        return short.Parse(_cellsValues[i].ToString());
	    }

	    public int GetInt32(int i)
	    {
	        if (IsDBNull(i)) return int.MinValue;

	        return int.Parse(_cellsValues[i].ToString());
	    }

	    public long GetInt64(int i)
	    {
	        if (IsDBNull(i)) return long.MinValue;

	        return long.Parse(_cellsValues[i].ToString());
	    }

	    public string GetString(int i)
	    {
	        if (IsDBNull(i)) return null;

	        return _cellsValues[i].ToString();
	    }

	    public object GetValue(int i)
	    {
	        return _cellsValues[i];
	    }

	    public bool IsDBNull(int i)
	    {
	        return (null == _cellsValues[i]) || (DBNull.Value == _cellsValues[i]);
	    }

	    public object this[int i]
	    {
	        get { return _cellsValues[i]; }
	    }

	    #endregion

	    #region IDisposable Members

	    public void Dispose()
	    {
	        Dispose(true);

	        GC.SuppressFinalize(this);
	    }

	    private void Dispose(bool disposing)
	    {
	        // Check to see if Dispose has already been called.
	        if (!disposed)
	        {
	            if (disposing)
	            {
	                if (_zipWorker != null) _zipWorker.Dispose();
	                if (_xmlReader != null) _xmlReader.Close();
	                if (_sheetStream != null) _sheetStream.Close();
	            }

	            _zipWorker = null;
	            _xmlReader = null;
	            _sheetStream = null;

	            _workbook = null;
	            _cellsValues = null;
	            _savedCellsValues = null;

	            disposed = true;
	        }
	    }

	    ~ExcelOpenXmlReader()
	    {
	        Dispose(false);
	    }

	    #endregion

	    #region  Not Supported IDataReader Members


	    public DataTable GetSchemaTable()
	    {
	        throw new NotSupportedException();
	    }

	    public int RecordsAffected
	    {
	        get { throw new NotSupportedException(); }
	    }

	    #endregion

	    #region Not Supported IDataRecord Members


	    public byte GetByte(int i)
	    {
	        throw new NotSupportedException();
	    }

	    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
	    {
	        throw new NotSupportedException();
	    }

	    public char GetChar(int i)
	    {
	        throw new NotSupportedException();
	    }

	    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
	    {
	        throw new NotSupportedException();
	    }

	    public IDataReader GetData(int i)
	    {
	        throw new NotSupportedException();
	    }

	    public string GetDataTypeName(int i)
	    {
	        throw new NotSupportedException();
	    }

	    public Type GetFieldType(int i)
	    {
	        throw new NotSupportedException();
	    }

	    public Guid GetGuid(int i)
	    {
	        throw new NotSupportedException();
	    }

	    public string GetName(int i)
	    {
	        throw new NotSupportedException();
	    }

	    public int GetOrdinal(string name)
	    {
	        throw new NotSupportedException();
	    }

	    public int GetValues(object[] values)
	    {
	        throw new NotSupportedException();
	    }

	    public object this[string name]
	    {
	        get { throw new NotSupportedException(); }
	    }

	    #endregion
	}
}
