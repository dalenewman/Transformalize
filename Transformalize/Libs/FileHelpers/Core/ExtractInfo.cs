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

namespace Transformalize.Libs.FileHelpers.Core
{
    internal sealed class ExtractedInfo
    {
        //public int CharsRemoved;
        //public string ExtractedString;

        internal static readonly ExtractedInfo Empty = new ExtractedInfo(string.Empty);
        public int ExtractedFrom;
        public int ExtractedTo;
        internal string mCustomExtractedString = null;
        public LineInfo mLine;

        //public int ExtraLines;
        //public string NewRestOfLine;
        //public string TrailString;

        public ExtractedInfo(LineInfo line)
        {
            mLine = line;
            ExtractedFrom = line.mCurrentPos;
            ExtractedTo = line.mLine.Length - 1;
            //CharsRemoved = ExtractedTo - ExtractedFrom + 1;
            //ExtraLines = 0;
            //	NewRestOfLine = null;
        }

        public ExtractedInfo(LineInfo line, int extractTo)
        {
            mLine = line;
            ExtractedFrom = line.mCurrentPos;
            ExtractedTo = extractTo - 1;
            //CharsRemoved = ExtractedTo - ExtractedFrom + 1;
            //ExtraLines = 0;
            //	NewRestOfLine = null;
        }

        public ExtractedInfo(string customExtract)
        {
            mCustomExtractedString = customExtract;
        }

        public int Length
        {
            get { return ExtractedTo - ExtractedFrom + 1; }
        }

        public string ExtractedString()
        {
            if (mCustomExtractedString == null)
                return new string(mLine.mLine, ExtractedFrom, ExtractedTo - ExtractedFrom + 1);
            else
                return mCustomExtractedString;
            //			return new string(mLine,  .mLine.Substring(ExtractedFrom, ExtractedTo - ExtractedFrom + 1);
        }

        public void TrimStart(char[] sortedToTrim)
        {
            if (mCustomExtractedString != null)
                mCustomExtractedString = mCustomExtractedString.TrimStart(sortedToTrim);
            else
                while (ExtractedFrom < ExtractedTo && Array.BinarySearch(sortedToTrim, mLine.mLine[ExtractedFrom]) >= 0)
                    ExtractedFrom++;
        }

        public void TrimEnd(char[] sortedToTrim)
        {
            if (mCustomExtractedString != null)
                mCustomExtractedString = mCustomExtractedString.TrimEnd(sortedToTrim);
            else
                while (ExtractedTo > ExtractedFrom && Array.BinarySearch(sortedToTrim, mLine.mLine[ExtractedTo]) >= 0)
                    ExtractedTo--;
        }

        public void TrimBoth(char[] sortedToTrim)
        {
            if (mCustomExtractedString != null)
                mCustomExtractedString = mCustomExtractedString.Trim(sortedToTrim);
            else
            {
                while (ExtractedFrom <= ExtractedTo && Array.BinarySearch(sortedToTrim, mLine.mLine[ExtractedFrom]) >= 0)
                {
                    ExtractedFrom++;
                }

                while (ExtractedTo > ExtractedFrom && Array.BinarySearch(sortedToTrim, mLine.mLine[ExtractedTo]) >= 0)
                {
                    ExtractedTo--;
                }
            }
        }

        //
        //				  public ExtractedInfo(string extracted, int charsRem, int lines)
        //				  {
        //					  ExtractedString = extracted;
        //					  CharsRemoved = charsRem;
        //					  ExtraLines = lines;
        //					  NewRestOfLine = null;
        //				  }

        public bool HasOnlyThisChars(char[] sortedArray)
        {
            // Check if the chars at pos or right are empty ones
            if (mCustomExtractedString != null)
            {
                var pos = 0;
                while (pos < mCustomExtractedString.Length &&
                       Array.BinarySearch(sortedArray, mCustomExtractedString[pos]) >= 0)
                {
                    pos++;
                }

                return pos == mCustomExtractedString.Length;
            }
            else
            {
                var pos = ExtractedFrom;
                while (pos <= ExtractedTo && Array.BinarySearch(sortedArray, mLine.mLine[pos]) >= 0)
                {
                    pos++;
                }

                return pos > ExtractedTo;
            }
        }
    }
}