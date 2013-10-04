#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.ExcelDataReader.Core.OpenXmlFormat
{
	internal class XlsxWorksheet
	{
		public const string N_dimension = "dimension";
		public const string N_row = "row";
		public const string N_c = "c";
		public const string N_v = "v";
		public const string A_ref = "ref";
		public const string A_r = "r";
		public const string A_t = "t";
		public const string A_s = "s";
	    private readonly string _Name;
	    private readonly int _id;

	    private XlsxDimension _dimension;

	    public XlsxWorksheet(string name, int id, string rid)
	    {
	        _Name = name;
	        _id = id;
	        RID = rid;
	    }

	    public XlsxDimension Dimension
		{
			get { return _dimension; }
			set { _dimension = value; }
		}

		public int ColumnsCount
		{
			get
			{
				return _dimension == null ? -1 : _dimension.LastCol - _dimension.FirstCol + 1;
			}
		}

		public int RowsCount
		{
			get
			{
				return _dimension == null ? -1 : _dimension.LastRow - _dimension.FirstRow + 1;
			}
		}

	    public string Name
		{
			get { return _Name; }
		}

	    public int Id
		{
			get { return _id; }
		}

	    public string RID { get; set; }

	    public string Path { get; set; }
	}
}
