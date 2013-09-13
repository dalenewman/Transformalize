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

using System;
using System.Globalization;
using Transformalize.Libs.FileHelpers.Attributes;
using Transformalize.Libs.FileHelpers.ErrorHandling;
using Transformalize.Libs.FileHelpers.Helpers;

namespace Transformalize.Libs.FileHelpers.Converters
{
    /// <summary>
    ///     Class that provides static methods that returns a default <see cref="ConverterBase">Converter</see> to the basic types.
    /// </summary>
    /// <remarks>
    ///     Used by the <see cref="FieldConverterAttribute" />.
    /// </remarks>
    internal sealed class ConvertHelpers
    {
        private const string DefaultDecimalSep = ".";

        #region "  Constructors  "

        // Not allow direct creation
        private ConvertHelpers()
        {
        }

        #endregion

        #region "  CreateCulture  "

        private static CultureInfo CreateCulture(string decimalSep)
        {
            var ci = new CultureInfo(CultureInfo.CurrentCulture.LCID);

            if (decimalSep == ".")
            {
                ci.NumberFormat.NumberDecimalSeparator = ".";
                ci.NumberFormat.NumberGroupSeparator = ",";
            }
            else if (decimalSep == ",")
            {
                ci.NumberFormat.NumberDecimalSeparator = ",";
                ci.NumberFormat.NumberGroupSeparator = ".";
            }
            else
                throw new BadUsageException("You can only use '.' or ',' as decimal or grup separators");

            return ci;
        }

        #endregion

        #region "  GetDefaultConverter  "

        internal static ConverterBase GetDefaultConverter(string fieldName, Type fieldType)
        {
#if NET_2_0

            if (fieldType.IsValueType &&
                  fieldType.IsGenericType &&
                    fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                fieldType = fieldType.GetGenericArguments()[0];
            }

#endif
            // Try to assign a default Converter
            if (fieldType == typeof (string))
                return null;
            else if (fieldType == typeof (Int16))
                return new Int16Converter();
            else if (fieldType == typeof (Int32))
                return new Int32Converter();
            else if (fieldType == typeof (Int64))
                return new Int64Converter();
            else if (fieldType == typeof (SByte))
                return new SByteConverter();
            else if (fieldType == typeof (UInt16))
                return new UInt16Converter();
            else if (fieldType == typeof (UInt32))
                return new UInt32Converter();
            else if (fieldType == typeof (UInt64))
                return new UInt64Converter();
            else if (fieldType == typeof (Byte))
                return new ByteConverter();
            else if (fieldType == typeof (Decimal))
                return new DecimalConverter();
            else if (fieldType == typeof (Double))
                return new DoubleConverter();
            else if (fieldType == typeof (Single))
                return new SingleConverter();
            else if (fieldType == typeof (DateTime))
                return new DateTimeConverter();
            else if (fieldType == typeof (Boolean))
                return new BooleanConverter();
#if ! MINI
            else if (fieldType.IsEnum)
                return new EnumConverter(fieldType);
#endif

            throw new BadUsageException("The field: '" + fieldName + "' of type: " + fieldType.Name + " is a non system type, so this field need a CustomConverter (see the docs for more info).");
        }

        #endregion

        internal sealed class ByteConverter : CultureConverter
        {
            public ByteConverter()
                : this(DefaultDecimalSep)
            {
            }

            public ByteConverter(string decimalSep)
                : base(typeof (Byte), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return Byte.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }

        internal abstract class CultureConverter
            : ConverterBase
        {
            protected CultureInfo mCulture;
            protected Type mType;

            public CultureConverter(Type T, string decimalSep)
            {
                mCulture = CreateCulture(decimalSep);
                mType = T;
            }

            public override sealed string FieldToString(object from)
            {
                return ((IConvertible) from).ToString(mCulture);
            }

            public override sealed object StringToField(string from)
            {
                object val;

                try
                {
                    val = ParseString(from);
                }
                catch
                {
                    throw new ConvertException(from, mType);
                }

                return val;
            }

            protected abstract object ParseString(string from);
        }


        internal sealed class UInt16Converter : CultureConverter
        {
            public UInt16Converter()
                : this(DefaultDecimalSep)
            {
            }

            public UInt16Converter(string decimalSep)
                : base(typeof (UInt16), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return UInt16.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }


        internal sealed class UInt32Converter : CultureConverter
        {
            public UInt32Converter()
                : this(DefaultDecimalSep)
            {
            }

            public UInt32Converter(string decimalSep)
                : base(typeof (UInt32), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return UInt32.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }


        internal sealed class UInt64Converter : CultureConverter
        {
            public UInt64Converter()
                : this(DefaultDecimalSep)
            {
            }

            public UInt64Converter(string decimalSep)
                : base(typeof (UInt64), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return UInt64.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }

        #region "  Int16, Int32, Int64 Converters  "

        #region "  Convert Classes  "

        internal sealed class Int16Converter : CultureConverter
        {
            public Int16Converter()
                : this(DefaultDecimalSep)
            {
            }

            public Int16Converter(string decimalSep)
                : base(typeof (Int16), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return Int16.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }

        internal sealed class Int32Converter : CultureConverter
        {
            public Int32Converter()
                : this(DefaultDecimalSep)
            {
            }


            public Int32Converter(string decimalSep)
                : base(typeof (Int32), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return Int32.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }

        internal sealed class Int64Converter : CultureConverter
        {
            public Int64Converter()
                : this(DefaultDecimalSep)
            {
            }

            public Int64Converter(string decimalSep)
                : base(typeof (Int64), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return Int64.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }

        internal sealed class SByteConverter : CultureConverter
        {
            public SByteConverter()
                : this(DefaultDecimalSep)
            {
            }

            public SByteConverter(string decimalSep)
                : base(typeof (SByte), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return SByte.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }

        #endregion

        #endregion

        #region "  Single, Double, DecimalConverters  "

        #region "  Convert Classes  "

        internal sealed class DecimalConverter : CultureConverter
        {
            public DecimalConverter()
                : this(DefaultDecimalSep)
            {
            }

            public DecimalConverter(string decimalSep)
                : base(typeof (Decimal), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return Decimal.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }


        internal sealed class DoubleConverter : CultureConverter
        {
            public DoubleConverter()
                : this(DefaultDecimalSep)
            {
            }

            public DoubleConverter(string decimalSep)
                : base(typeof (Double), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return Double.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }

        internal sealed class SingleConverter : CultureConverter
        {
            public SingleConverter()
                : this(DefaultDecimalSep)
            {
            }

            public SingleConverter(string decimalSep)
                : base(typeof (Single), decimalSep)
            {
            }

            protected override object ParseString(string from)
            {
                return Single.Parse(StringHelper.RemoveBlanks(from), NumberStyles.Number, mCulture);
            }
        }

        #endregion

        #endregion

        #region "  Date Converters  "

        #region "  Convert Classes  "

        internal sealed class DateTimeConverter : ConverterBase
        {
            private readonly string mFormat;

            public DateTimeConverter() : this(DefaultDateTimeFormat)
            {
            }

            public DateTimeConverter(string format)
            {
                if (format == null || format == String.Empty)
                    throw new BadUsageException("The format of the DateTime Converter can be null or empty.");

                try
                {
                    var tmp = DateTime.Now.ToString(format);
                }
                catch
                {
                    throw new BadUsageException("The format: '" + format + " is invalid for the DateTime Converter.");
                }

                mFormat = format;
            }

            //static CultureInfo mInvariant = System.Globalization.CultureInfo.InvariantCulture;

            public override object StringToField(string from)
            {
                if (from == null) from = string.Empty;

                object val;
                try
                {
                    val = DateTime.ParseExact(from.Trim(), mFormat, null);
                }
                catch
                {
                    var extra = String.Empty;
                    if (from.Length > mFormat.Length)
                        extra = " There are more chars than in the format string: '" + mFormat + "'";
                    else if (from.Length < mFormat.Length)
                        extra = " There are less chars than in the format string: '" + mFormat + "'";
                    else
                        extra = " Using the format: '" + mFormat + "'";


                    throw new ConvertException(from, typeof (DateTime), extra);
                }
                return val;
            }

            public override string FieldToString(object from)
            {
                return Convert.ToDateTime(from).ToString(mFormat);
            }
        }

        #endregion

        #endregion

        #region "  Boolean Converters  "

        #region "  Convert Classes  "

        internal sealed class BooleanConverter : ConverterBase
        {
            private readonly string mFalseString;
            private readonly string mFalseStringLower;
            private readonly string mTrueString;
            private readonly string mTrueStringLower;

            public BooleanConverter()
            {
            }

            public BooleanConverter(string trueStr, string falseStr)
            {
                mTrueString = trueStr;
                mFalseString = falseStr;
                mTrueStringLower = trueStr.ToLower();
                mFalseStringLower = falseStr.ToLower();
            }

            public override object StringToField(string from)
            {
                object val;
                try
                {
                    var testTo = from.ToLower();

                    if (mTrueString == null)
                    {
                        testTo = testTo.Trim();
                        if (testTo == "true" || testTo == "1")
                            val = true;
                        else if (testTo == "false" || testTo == "0" || testTo == "")
                            val = false;
                        else
                            throw new Exception();
                    }
                    else
                    {
                        if (testTo == mTrueStringLower || testTo.Trim() == mTrueStringLower)
                            val = true;
                        else if (testTo == mFalseStringLower || testTo.Trim() == mFalseStringLower)
                            val = false;
                        else
                            throw new ConvertException(from, typeof (bool), "The string: " + from + " cant be recognized as boolean using the true/false values: " + mTrueString + "/" + mFalseString);
                    }
                }
                catch
                {
                    throw new ConvertException(from, typeof (Boolean));
                }

                return val;
            }

            public override string FieldToString(object from)
            {
                var b = Convert.ToBoolean(from);
                if (b)
                    if (mTrueString == null)
                        return "True";
                    else
                        return mTrueString;
                else if (mFalseString == null)
                    return "False";
                else
                    return mFalseString;
            }
        }

        #endregion

        #endregion
    }
}