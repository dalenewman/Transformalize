#region License
// /*
// See license included in this library folder.
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