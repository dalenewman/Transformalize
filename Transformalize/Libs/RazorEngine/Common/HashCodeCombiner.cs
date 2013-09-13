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

using System.Collections;

namespace Transformalize.Libs.RazorEngine.Common
{
    internal class HashCodeCombiner
    {
        private long _combinedHash64 = 0x1505L;

        public int CombinedHash
        {
            get { return _combinedHash64.GetHashCode(); }
        }

        public HashCodeCombiner Add(IEnumerable e)
        {
            if (e == null)
            {
                Add(0);
            }
            else
            {
                var count = 0;
                foreach (var o in e)
                {
                    Add(o);
                    count++;
                }
                Add(count);
            }
            return this;
        }

        public HashCodeCombiner Add(int i)
        {
            _combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;
            return this;
        }

        public HashCodeCombiner Add(object o)
        {
            var hashCode = (o != null) ? o.GetHashCode() : 0;
            Add(hashCode);
            return this;
        }

        public static HashCodeCombiner Start()
        {
            return new HashCodeCombiner();
        }
    }
}