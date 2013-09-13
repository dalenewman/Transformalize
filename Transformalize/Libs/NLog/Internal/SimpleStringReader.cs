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

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Simple character tokenizer.
    /// </summary>
    internal class SimpleStringReader
    {
        private readonly string text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleStringReader" /> class.
        /// </summary>
        /// <param name="text">The text to be tokenized.</param>
        public SimpleStringReader(string text)
        {
            this.text = text;
            Position = 0;
        }

        internal int Position { get; set; }

        internal string Text
        {
            get { return text; }
        }

        internal int Peek()
        {
            if (Position < text.Length)
            {
                return text[Position];
            }

            return -1;
        }

        internal int Read()
        {
            if (Position < text.Length)
            {
                return text[Position++];
            }

            return -1;
        }

        internal string Substring(int p0, int p1)
        {
            return text.Substring(p0, p1 - p0);
        }
    }
}