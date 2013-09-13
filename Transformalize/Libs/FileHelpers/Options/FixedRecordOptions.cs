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

using Transformalize.Libs.FileHelpers.Attributes;
using Transformalize.Libs.FileHelpers.Core;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.Fields;

namespace Transformalize.Libs.FileHelpers.Options
{
    /// <summary>
    ///     This class allows you to set some options of the fixed length records but at runtime.
    ///     With this options the library is more flexible than never.
    /// </summary>
    public sealed class FixedRecordOptions : RecordOptions
    {
        internal FixedRecordOptions(RecordInfo info)
            : base(info)
        {
        }


        /// <summary>
        ///     Indicates the behavior when variable length records are found in a [<see cref="FixedLengthRecordAttribute" />]. (Note: nothing in common with [FieldOptional])
        /// </summary>
        public FixedMode FixedMode
        {
            get { return ((FixedLengthField) mRecordInfo.mFields[0]).mFixedMode; }
            set
            {
                for (var i = 0; i < mRecordInfo.mFieldCount; i++)
                {
                    ((FixedLengthField) mRecordInfo.mFields[i]).mFixedMode = value;
                }
            }
        }

#if NET_2_0
        [DebuggerDisplay("FileHelperEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
#endif
        private int mRecordLength = int.MinValue;

        /// <summary>
        ///     The sum of the indivial field lengths.
        /// </summary>
        public int RecordLength
        {
            get
            {
                if (mRecordLength != int.MinValue)
                    return mRecordLength;

                mRecordLength = 0;
                foreach (FixedLengthField field in mRecordInfo.mFields)
                {
                    mRecordLength += field.mFieldLength;
                }

                return mRecordLength;
            }
        }
    }
}