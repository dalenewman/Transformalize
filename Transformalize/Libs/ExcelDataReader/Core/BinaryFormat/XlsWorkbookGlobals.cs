#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;

namespace Transformalize.Libs.ExcelDataReader.Core.BinaryFormat
{
	/// <summary>
	/// Represents Globals section of workbook
	/// </summary>
	internal class XlsWorkbookGlobals
	{
		private readonly List<XlsBiffRecord> m_ExtendedFormats = new List<XlsBiffRecord>();
		private readonly List<XlsBiffRecord> m_Fonts = new List<XlsBiffRecord>();
		private readonly List<XlsBiffRecord> m_Formats = new List<XlsBiffRecord>();
		private readonly List<XlsBiffBoundSheet> m_Sheets = new List<XlsBiffBoundSheet>();
		private readonly List<XlsBiffRecord> m_Styles = new List<XlsBiffRecord>();

	    public XlsBiffInterfaceHdr InterfaceHdr { get; set; }

	    public XlsBiffRecord MMS { get; set; }

	    public XlsBiffRecord WriteAccess { get; set; }

	    public XlsBiffSimpleValueRecord CodePage { get; set; }

	    public XlsBiffRecord DSF { get; set; }

	    public XlsBiffRecord Country { get; set; }

	    public XlsBiffSimpleValueRecord Backup { get; set; }

	    public List<XlsBiffRecord> Fonts
		{
			get { return m_Fonts; }
		}

		public List<XlsBiffRecord> Formats
		{
			get { return m_Formats; }
		}

		public List<XlsBiffRecord> ExtendedFormats
		{
			get { return m_ExtendedFormats; }
		}

		public List<XlsBiffRecord> Styles
		{
			get { return m_Styles; }
		}

		public List<XlsBiffBoundSheet> Sheets
		{
			get { return m_Sheets; }
		}

	    /// <summary>
	    /// Shared String Table of workbook
	    /// </summary>
	    public XlsBiffSST SST { get; set; }

	    public XlsBiffRecord ExtSST { get; set; }
	}
}