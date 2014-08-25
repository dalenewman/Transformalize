using System;

namespace Transformalize.Main {
    public static class DataCaster {
        public static void Cast(ref Data data, byte type, object value) {
            if (value == null)
                return;
            switch (type) {
                case 0:
                    data.String = (string)value;
                    break;
                case 1:
                    data.Short = (short)value;
                    break;
                case 2:
                    data.Int = (int)value;
                    break;
                case 3:
                    data.Long = (long)value;
                    break;
                case 4:
                    data.Double = (double)value;
                    break;
                case 5:
                    data.Decimal = (decimal)value;
                    break;
                case 6:
                    data.Char = (char)value;
                    break;
                case 7:
                    data.DateTime = (DateTime)value;
                    break;
                case 8:
                    data.Bool = (bool)value;
                    break;
                case 9:
                    data.Single = (Single)value;
                    break;
                case 10:
                    data.Guid = (Guid)value;
                    break;
                case 11:
                    data.Byte = (byte)value;
                    break;
                case 12:
                    data.Binary = (byte[])value;
                    break;
                case 13:
                    data.UInt64 = (UInt64)value;
                    break;
                case 14:
                    data.Object = value;
                    break;
            }
        }
    }
}