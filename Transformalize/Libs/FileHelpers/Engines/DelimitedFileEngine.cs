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
using Transformalize.Libs.FileHelpers.ErrorHandling;
using Transformalize.Libs.FileHelpers.Fields;
using Transformalize.Libs.FileHelpers.Options;

namespace Transformalize.Libs.FileHelpers.Engines
{
    /// <summary>
    ///     Is a version of the <see cref="FileHelperEngine" /> exclusive for
    ///     delimited records that allow you to change the delimiter an other options at runtime
    /// </summary>
    /// <remarks>
    ///     Useful when you need to export or import the same info with 2 or more different delimiters or little different options.
    /// </remarks>
#if NET_2_0
    [DebuggerDisplay("DelimitedFileEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
#endif
    public sealed class DelimitedFileEngine : FileHelperEngine
    {
        /// <summary>
        ///     Create a version of the <see cref="FileHelperEngine" /> exclusive for
        ///     delimited records that allow you to change the delimiter an other options at runtime
        /// </summary>
        /// <remarks>
        ///     Useful when you need to export or import the same info with 2 or more different delimiters.
        /// </remarks>
        /// <param name="recordType">The record mapping class.</param>
        public DelimitedFileEngine(Type recordType)
            : base(recordType)
        {
            if (mRecordInfo.mFields[0] is DelimitedField == false)
                throw new BadUsageException("The Delimited Engine only accepts record types marked with DelimitedRecordAttribute");
        }


        /// <summary>Allow changes in the record layout like delimiters and others common settings.</summary>
        public new DelimitedRecordOptions Options
        {
            get { return (DelimitedRecordOptions) mOptions; }
        }
    }


#if NET_2_0

    /// <summary>
    /// Is a version of the <see cref="FileHelperEngine"/> exclusive for 
    /// delimited records that allow you to change the delimiter an other options at runtime
    /// </summary>
    /// <remarks>
    /// Useful when you need to export or import the same info with 2 or more different delimiters or little different options.
    /// </remarks>
#if NET_2_0
    [DebuggerDisplay("DelimitedFileEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
#endif
    public sealed class DelimitedFileEngine<T> : FileHelperEngine<T>
	{
	#region "  Constructor  "

		/// <summary>
		/// Create a version of the <see cref="FileHelperEngine"/> exclusive for 
		/// delimited records that allow you to change the delimiter an other options at runtime
		/// </summary>
		/// <remarks>
		/// Useful when you need to export or import the same info with 2 or more different delimiters.
		/// </remarks>
		public DelimitedFileEngine()
			: base()
		{
			if (mRecordInfo.mFields[0] is DelimitedField == false)
				throw new BadUsageException("The Delimited Engine only accepts Record Types marked with DelimitedRecordAttribute");
		}

	#endregion

		
		/// <summary>Allow changes in the record layout like delimiters and others common settings.</summary>
        public new DelimitedRecordOptions Options
		{
            get { return (DelimitedRecordOptions) mOptions; }
			
		}
	}

#endif
}