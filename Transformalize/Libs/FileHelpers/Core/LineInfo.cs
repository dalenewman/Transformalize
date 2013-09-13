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

namespace Transformalize.Libs.FileHelpers.Core
{
    internal sealed class LineInfo
    {
        #region "  Constructor  "

        private static readonly char[] mEmptyChars = new char[] {};

        public LineInfo(string line)
        {
            mLineStr = line;
            mLine = line == null ? mEmptyChars : line.ToCharArray();
            mCurrentPos = 0;
        }

        #endregion

        #region "  Internal Fields  "

        //internal string  mLine;
        private static readonly char[] WhitespaceChars = new[]
                                                             {
                                                                 '\t', '\n', '\v', '\f', '\r', ' ', '\x00a0', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008',
                                                                 '\u2009', '\u200a', '\u200b', '\u3000', '\ufeff'
                                                             };

        internal int mCurrentPos;

        internal char[] mLine;
        internal string mLineStr;
        internal ForwardReader mReader;

        #endregion

        private static readonly CompareInfo mCompare = CultureInfo.InvariantCulture.CompareInfo;

        public string CurrentString
        {
            get { return new string(mLine, mCurrentPos, mLine.Length - mCurrentPos); }
        }

        public int CurrentLength
        {
            get { return mLine.Length - mCurrentPos; }
        }

        public bool IsEOL()
        {
            return mCurrentPos >= mLine.Length;
        }

        public bool EmptyFromPos()
        {
            // Chek if the chars at pos or right are empty ones
            var length = mLine.Length;
            var pos = mCurrentPos;
            while (pos < length && Array.BinarySearch(WhitespaceChars, mLine[pos]) >= 0)
            {
                pos++;
            }

            return pos >= length;
        }

        public void TrimStart()
        {
            TrimStartSorted(WhitespaceChars);
        }

        public void TrimStart(char[] toTrim)
        {
            Array.Sort(toTrim);
            TrimStartSorted(toTrim);
        }

        private void TrimStartSorted(char[] toTrim)
        {
            // Move the pointer to the first non to Trim char
            var length = mLine.Length;

            while (mCurrentPos < length && Array.BinarySearch(toTrim, mLine[mCurrentPos]) >= 0)
            {
                mCurrentPos++;
            }
        }

        public bool StartsWith(string str)
        {
            // Returns true if the string begin with str
            if (mCurrentPos >= mLineStr.Length)
                return false;
            else
                return mCompare.Compare(mLineStr, mCurrentPos, str.Length, str, 0, str.Length, CompareOptions.IgnoreCase) == 0;
        }

        public bool StartsWithTrim(string str)
        {
            var length = mLine.Length;
            var pos = mCurrentPos;

            while (pos < length && Array.BinarySearch(WhitespaceChars, mLine[pos]) >= 0)
            {
                pos++;
            }

            return mCompare.Compare(mLineStr, pos, str, 0, CompareOptions.IgnoreCase) == 0;
        }

        public void ReadNextLine()
        {
            mLineStr = mReader.ReadNextLine();
            mLine = mLineStr.ToCharArray();

            mCurrentPos = 0;
        }


        public int IndexOf(string foundThis)
        {
            // Bad performance with custom IndexOf
            //			if (foundThis.Length == 1)
            //			{
            //				char delimiter = foundThis[0];
            //				int pos = mCurrentPos;
            //				int length = mLine.Length;
            //			
            //				while (pos < length)
            //				{
            //					if (mLine[pos] == delimiter)
            //						return pos;
            //				
            //					pos++;
            //				}
            //				return -1;
            //			}
            //			else
//			if (mLineStr == null)
//				return -1;
//			else
            return mCompare.IndexOf(mLineStr, foundThis, mCurrentPos, CompareOptions.IgnoreCase);
        }

        internal void ReLoad(string line)
        {
            mLine = line == null ? mEmptyChars : line.ToCharArray();
            mLineStr = line;
            mCurrentPos = 0;
        }
    }
}