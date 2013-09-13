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

namespace Transformalize.Libs.FileHelpers.Enums
{
    /// <summary>
    ///     Indicates the convertion used in the
    ///     <see
    ///         cref="T:Transformalize.Libs.FileHelpers.Attributes.FieldConverterAttribute" />
    ///     .
    /// </summary>
    public enum ConverterKind
    {
        /// <summary>Null Converter.</summary>
        None = 0,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Date</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>string</b> with the date format.
        ///     </para>
        /// </summary>
        Date,

        /// <summary>
        ///     Convert from/to <b>Boolean</b> values.
        /// </summary>
        Boolean,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Byte</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        Byte,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Int16 or short</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        Int16,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Int32 or int</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        Int32,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Int64 or long</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        Int64,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Decimal</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        Decimal,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Double</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        Double,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Single</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        Single,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>Byte</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        SByte,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>UInt16 or unsigned short</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        UInt16,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>UInt32 or unsigned int</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        UInt32,

        /// <summary>
        ///     <para>
        ///         Convert from/to <b>UInt64 or unsigned long</b> values.
        ///     </para>
        ///     <para>
        ///         Params: arg1 is the <b>decimal separator</b>, by default '.'
        ///     </para>
        /// </summary>
        UInt64
    }
}