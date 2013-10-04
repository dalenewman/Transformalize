#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.IO;
using System.Text;
using Transformalize.Libs.FileHelpers.Helpers;

namespace Transformalize.Libs.FileHelpers.Core
{
    internal sealed class ForwardReader
    {
        private readonly int mFowardLines;
        private readonly string[] mFowardStrings;
        private readonly TextReader mReader;
        internal int mCapacityHint = 64;
        private bool mDiscardForward;
        internal char[] mEOF = StringHelper.NewLine.ToCharArray();
        private int mForwardIndex;

        private int mLineNumber;
        private int mRemaingLines;

//		
//		int mPos = 0;
//		int MaxRecordSize = 1024 * 8;
//		char[] mBuffer;
//		
        internal ForwardReader(TextReader reader)
            : this(reader, 0, 0)
        {
        }

        internal ForwardReader(TextReader reader, int forwardLines) :
            this(reader, forwardLines, 0)
        {
        }

        internal ForwardReader(TextReader reader, int forwardLines, int startLine)
        {
            mReader = reader;

            mFowardLines = forwardLines;
            mLineNumber = startLine;

            mFowardStrings = new string[mFowardLines + 1];
            mRemaingLines = mFowardLines + 1;

            for (var i = 0; i < mFowardLines + 1; i++)
            {
                mFowardStrings[i] = mReader.ReadLine();
                mLineNumber++;
                if (mFowardStrings[i] == null)
                {
                    mRemaingLines = i;
                    break;
                }
            }
        }

        public int RemainingLines
        {
            get { return mRemaingLines; }
        }

        public int LineNumber
        {
            get { return mLineNumber - 1; }
        }


//		public string ReadToDelimiter(string del)
//		{
//			//StringBuilder builder = new StringBuilder(mCapacityHint);
//
//			int right = mPos;
//			while (true)
//			{
//				mReader.
//				
//				//mReader.Read()
//				
//			}
//			
//			
//			
//			if (builder.Length > 0)
//			{
//				return builder.ToString();
//			}
//			return null;
//		}

        public bool DiscardForward
        {
            get { return mDiscardForward; }
            set { mDiscardForward = value; }
        }

        public int FowardLines
        {
            get { return mFowardLines; }
        }

        public string RemainingText
        {
            get
            {
                var sb = new StringBuilder(100);

                for (var i = 0; i < mRemaingLines + 1; i++)
                {
                    sb.Append(mFowardStrings[(mForwardIndex + i)%(mFowardLines + 1)] + StringHelper.NewLine);
                }

                return sb.ToString();
            }
        }

        public string ReadNextLine()
        {
            if (mRemaingLines <= 0)
                return null;
            else
            {
                var res = mFowardStrings[mForwardIndex];

                if (mRemaingLines == (mFowardLines + 1))
                {
                    mFowardStrings[mForwardIndex] = mReader.ReadLine();
                    mLineNumber++;

                    if (mFowardStrings[mForwardIndex] == null)
                    {
                        mRemaingLines--;
                    }
                }
                else
                {
                    mRemaingLines--;
                    if (mDiscardForward)
                        return null;
                }

                mForwardIndex = (mForwardIndex + 1)%(mFowardLines + 1);

                return res;
            }
        }


        public void Close()
        {
            mReader.Close();
        }
    }
}