using OpenXmlPowerTools;
using Pipeline.Configuration;

namespace Pipeline.Provider.OpenXml {
    public static class OpenXmlExtensions {
        public static CellDataType ToCellDataType(this Field f) {
            if (f.Type.StartsWith("bool"))
                return CellDataType.Boolean;

            if (f.Type.StartsWith("date"))
                return CellDataType.Date;

            return f.IsNumeric() ? CellDataType.Number : CellDataType.String;
        }
    }
}