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
    ///     fixed length records that allow you to change the delimiter an other options at runtime
    /// </summary>
    /// <remarks>
    ///     Useful when you need to export or import the same info with little different options.
    /// </remarks>
#if NET_2_0
    [DebuggerDisplay("FixedFileEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
#endif
    public sealed class FixedFileEngine : FileHelperEngine
    {
        #region "  Constructor  "

        /// <summary>
        ///     Creates a version of the <see cref="FileHelperEngine" /> exclusive for
        ///     fixed length records that allow you to change the delimiter an other options at runtime
        /// </summary>
        /// <remarks>
        ///     Useful when you need to export or import the same info with little different options.
        /// </remarks>
        /// <param name="recordType">The record mapping class.</param>
        public FixedFileEngine(Type recordType)
            : base(recordType)
        {
            if (mRecordInfo.mFields[0] is FixedLengthField == false)
                throw new BadUsageException("The FixedFileEngine only accepts Record Types marked with FixedLengthRecord attribute");
        }

        #endregion

        /// <summary>Allow changes some fixed length options and others common settings.</summary>
        public new FixedRecordOptions Options
        {
            get { return (FixedRecordOptions) mOptions; }
        }
    }


#if NET_2_0

    /// <summary>
    /// Is a version of the <see cref="FileHelperEngine"/> exclusive for 
    /// fixed length records that allow you to change the delimiter an other options at runtime
    /// </summary>
    /// <remarks>
    /// Useful when you need to export or import the same info with little different options.
    /// </remarks>
#if NET_2_0
    [DebuggerDisplay("FixedFileEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
#endif
    public sealed class FixedFileEngine<T> : FileHelperEngine<T>
	{
	#region "  Constructor  "

		/// <summary>
		/// Creates a version of the <see cref="FileHelperEngine"/> exclusive for 
		/// fixed length records that allow you to change the delimiter an other options at runtime
		/// </summary>
		/// <remarks>
		/// Useful when you need to export or import the same info with little different options.
		/// </remarks>
		public FixedFileEngine()
			: base()
		{
			if (mRecordInfo.mFields[0] is FixedLengthField  == false)
				throw new BadUsageException("The FixedFileEngine only accepts Record Types marked with FixedLengthRecord attribute");
		}

	#endregion

		
		/// <summary>Allow changes some fixed length options and others common settings.</summary>
		public new FixedRecordOptions Options
		{
            get { return (FixedRecordOptions) mOptions; }
		}
	}
#endif
}