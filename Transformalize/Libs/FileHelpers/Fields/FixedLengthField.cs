#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Reflection;
using System.Text;
using Transformalize.Libs.FileHelpers.Attributes;
using Transformalize.Libs.FileHelpers.Core;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.Fields
{
    internal sealed class FixedLengthField : FieldBase
    {
        #region "  Properties  "

        internal FieldAlignAttribute mAlign = new FieldAlignAttribute(AlignMode.Left, ' ');
        internal int mFieldLength;

        internal FixedMode mFixedMode = FixedMode.ExactLength;

        #endregion

        #region "  Constructor  "

        internal FixedLengthField(FieldInfo fi, int length, FieldAlignAttribute align) : base(fi)
        {
            mFieldLength = length;

            if (align != null)
                mAlign = align;
            else
            {
                if (fi.FieldType == typeof (Int16) ||
                    fi.FieldType == typeof (Int32) ||
                    fi.FieldType == typeof (Int64) ||
                    fi.FieldType == typeof (UInt16) ||
                    fi.FieldType == typeof (UInt32) ||
                    fi.FieldType == typeof (UInt64) ||
                    fi.FieldType == typeof (byte) ||
                    fi.FieldType == typeof (sbyte) ||
                    fi.FieldType == typeof (decimal) ||
                    fi.FieldType == typeof (float) ||
                    fi.FieldType == typeof (double))

                    mAlign = new FieldAlignAttribute(AlignMode.Right, ' ');
            }
        }

        #endregion

        #region "  Overrides String Handling  "

        protected override ExtractedInfo ExtractFieldString(LineInfo line)
        {
            if (line.CurrentLength == 0)
            {
                if (mIsOptional)
                    return ExtractedInfo.Empty;
                else
                    throw new BadUsageException("End Of Line found processing the field: " + mFieldInfo.Name + " at line " + line.mReader.LineNumber.ToString() + ". (You need to mark it as [FieldOptional] if you want to avoid this exception)");
            }

            ExtractedInfo res;

            if (line.CurrentLength < mFieldLength)
                if (mFixedMode == FixedMode.AllowLessChars || mFixedMode == FixedMode.AllowVariableLength)
                    res = new ExtractedInfo(line);
                else
                    throw new BadUsageException("The string '" + line.CurrentString + "' (length " + line.CurrentLength.ToString() + ") at line " + line.mReader.LineNumber.ToString() + " has less chars than the defined for " + mFieldInfo.Name + " (" + mFieldLength.ToString() + "). You can use the [FixedLengthRecord(FixedMode.AllowLessChars)] to avoid this problem.");
            else if (mIsLast && line.CurrentLength > mFieldLength && mFixedMode != FixedMode.AllowMoreChars && mFixedMode != FixedMode.AllowVariableLength)
                throw new BadUsageException("The string '" + line.CurrentString + "' (length " + line.CurrentLength.ToString() + ") at line " + line.mReader.LineNumber.ToString() + " has more chars than the defined for the last field " + mFieldInfo.Name + " (" + mFieldLength.ToString() + ").You can use the [FixedLengthRecord(FixedMode.AllowMoreChars)] to avoid this problem.");
            else
                res = new ExtractedInfo(line, line.mCurrentPos + mFieldLength);

            return res;
        }

        protected override void CreateFieldString(StringBuilder sb, object fieldValue)
        {
            var field = base.BaseFieldString(fieldValue);

            if (field.Length > mFieldLength)
                field = field.Substring(0, mFieldLength);
            //sb.Length = length + this.mFieldLength;

            if (mAlign.Align == AlignMode.Left)
            {
                sb.Append(field);
                sb.Append(mAlign.AlignChar, mFieldLength - field.Length);
            }
            else if (mAlign.Align == AlignMode.Right)
            {
                sb.Append(mAlign.AlignChar, mFieldLength - field.Length);
                sb.Append(field);
            }
            else
            {
                var middle = (mFieldLength - field.Length)/2;

                sb.Append(mAlign.AlignChar, middle);
                sb.Append(field);
                sb.Append(mAlign.AlignChar, mFieldLength - field.Length - middle);
//				if (middle > 0)
//					res = res.PadLeft(mFieldLength - middle, mAlign.AlignChar).PadRight(mFieldLength, mAlign.AlignChar);
            }
        }

        #endregion
    }
}