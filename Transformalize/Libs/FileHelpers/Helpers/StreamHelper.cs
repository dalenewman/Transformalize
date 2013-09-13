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

using System.IO;
using System.Text;

namespace Transformalize.Libs.FileHelpers.Helpers
{
    internal static class StreamHelper
    {
        internal static TextWriter CreateFileAppender(string fileName, Encoding encode, bool correctEnd)
        {
            return CreateFileAppender(fileName, encode, correctEnd, true);
        }

        internal static TextWriter CreateFileAppender(string fileName, Encoding encode, bool correctEnd, bool disposeStream)
        {
            TextWriter res;

            if (correctEnd)
            {
                FileStream fs = null;

                try
                {
                    fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                    if (fs.Length >= 2)
                    {
                        fs.Seek(-2, SeekOrigin.End);

                        if (fs.ReadByte() == 13)
                        {
                            if (fs.ReadByte() == 10)
                            {
                                int nowRead;
                                do
                                {
                                    fs.Seek(-2, SeekOrigin.Current);
                                    nowRead = fs.ReadByte();
                                } while (nowRead == 13 || nowRead == 10);
                            }
                        }
                        else
                            fs.ReadByte();

                        fs.WriteByte(13);
                        fs.WriteByte(10);
                    }

                    res = new StreamWriter(fs, encode);
                }
                finally
                {
                    if (disposeStream && fs != null)
                        fs.Close();
                }
            }
            else
            {
                res = new StreamWriter(fileName, true, encode);
            }

            return res;
        }
    }
}