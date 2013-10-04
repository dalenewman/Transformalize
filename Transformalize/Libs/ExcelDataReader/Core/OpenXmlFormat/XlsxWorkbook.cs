#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Transformalize.Libs.ExcelDataReader.Core.OpenXmlFormat
{
    internal class XlsxWorkbook
    {
        private const string N_sheet = "sheet";
        private const string N_t = "t";
        private const string N_si = "si";
        private const string N_cellXfs = "cellXfs";
        private const string N_numFmts = "numFmts";

        private const string A_sheetId = "sheetId";
        private const string A_name = "name";
        private const string A_rid = "r:id";

        private const string N_rel = "Relationship";
        private const string A_id = "Id";
        private const string A_target = "Target";
        private XlsxSST _SST;
        private XlsxStyles _Styles;
        private List<XlsxWorksheet> sheets;

        private XlsxWorkbook() { }

        public XlsxWorkbook(Stream workbookStream, Stream relsStream, Stream sharedStringsStream, Stream stylesStream)
        {
            if (null == workbookStream) throw new ArgumentNullException();

            ReadWorkbook(workbookStream);

            ReadWorkbookRels(relsStream);

            ReadSharedStrings(sharedStringsStream);

            ReadStyles(stylesStream);
        }

        public List<XlsxWorksheet> Sheets
        {
            get { return sheets; }
            set { sheets = value; }
        }

        public XlsxSST SST
        {
            get { return _SST; }
        }

        public XlsxStyles Styles
        {
            get { return _Styles; }
        }


        private void ReadStyles(Stream xmlFileStream)
        {
            if (null == xmlFileStream) return;

            _Styles = new XlsxStyles();

            var rXlsxNumFmt = false;

            using (var reader = XmlReader.Create(xmlFileStream))
            {
                while (reader.Read())
                {
                    if (!rXlsxNumFmt && reader.NodeType == XmlNodeType.Element && reader.Name == N_numFmts)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1) break;

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == XlsxNumFmt.N_numFmt)
                            {
                                _Styles.NumFmts.Add(
                                    new XlsxNumFmt(
                                        int.Parse(reader.GetAttribute(XlsxNumFmt.A_numFmtId)),
                                        reader.GetAttribute(XlsxNumFmt.A_formatCode)
                                        ));
                            }
                        }

                        rXlsxNumFmt = true;
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == N_cellXfs)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1) break;

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == XlsxXf.N_xf)
                            {
                                _Styles.CellXfs.Add(
                                    new XlsxXf(
                                        int.Parse(reader.GetAttribute(XlsxXf.A_xfId)),
                                        int.Parse(reader.GetAttribute(XlsxXf.A_numFmtId)),
                                        reader.GetAttribute(XlsxXf.A_applyNumberFormat)
                                        ));
                            }
                        }

                        break;
                    }
                }

                xmlFileStream.Close();
            }
        }

        private void ReadSharedStrings(Stream xmlFileStream)
        {
            if (null == xmlFileStream) return;

            _SST = new XlsxSST();

            using (var reader = XmlReader.Create(xmlFileStream))
            {
                // There are multiple <t> in a <si>. Concatenate <t> within an <si>.
                var bAddStringItem = false;
                var sStringItem = "";

                while (reader.Read())
                {
                    // There are multiple <t> in a <si>. Concatenate <t> within an <si>.
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == N_si)
                    {
                        // Do not add the string item until the next string item is read.
                        if (bAddStringItem)
                        {
                            // Add the string item to XlsxSST.
                            _SST.Add(sStringItem);
                        }
                        else
                        {
                            // Add the string items from here on.
                            bAddStringItem = true;
                        }

                        // Reset the string item.
                        sStringItem = "";
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == N_t)
                    {
                        // Append to the string item.
                        sStringItem += reader.ReadElementContentAsString();
                    }
                }
                // Do not add the last string item unless we have read previous string items.
                if (bAddStringItem)
                {
                    // Add the string item to XlsxSST.
                    _SST.Add(sStringItem);
                }

                xmlFileStream.Close();
            }
        }


        private void ReadWorkbook(Stream xmlFileStream)
        {
            sheets = new List<XlsxWorksheet>();

            using (var reader = XmlReader.Create(xmlFileStream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == N_sheet)
                    {
                        sheets.Add(new XlsxWorksheet(
                                               reader.GetAttribute(A_name),
                                               int.Parse(reader.GetAttribute(A_sheetId)), reader.GetAttribute(A_rid)));
                    }

                }

                xmlFileStream.Close();
            }

        }

        private void ReadWorkbookRels(Stream xmlFileStream)
        {
            using (var reader = XmlReader.Create(xmlFileStream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == N_rel)
                    {
                        var rid = reader.GetAttribute(A_id);

                        for (var i = 0; i < sheets.Count; i++)
                        {
                            var tempSheet = sheets[i];

                            if (tempSheet.RID == rid)
                            {
                                tempSheet.Path = reader.GetAttribute(A_target);
                                sheets[i] = tempSheet;
                                break;
                            }
                        }
                    }

                }

                xmlFileStream.Close();
            }
        }

    }
}
