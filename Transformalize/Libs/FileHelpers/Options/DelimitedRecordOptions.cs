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

using Transformalize.Libs.FileHelpers.Core;
using Transformalize.Libs.FileHelpers.Fields;

namespace Transformalize.Libs.FileHelpers.Options
{
    /// <summary>
    ///     This class allows you to set some options of the delimited records but at runtime.
    ///     With this options the library is more flexible than never.
    /// </summary>
    public sealed class DelimitedRecordOptions : RecordOptions
    {
        internal DelimitedRecordOptions(RecordInfo info)
            : base(info)
        {
        }

        /// <summary>
        ///     The delimiter used to identify each field in the data.
        /// </summary>
        public string Delimiter
        {
            get { return ((DelimitedField) mRecordInfo.mFields[0]).Separator; }
            set
            {
                for (var i = 0; i < mRecordInfo.mFieldCount; i++)
                    ((DelimitedField) mRecordInfo.mFields[i]).Separator = value;
            }
        }
    }
}