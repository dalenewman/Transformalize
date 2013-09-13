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

namespace Transformalize.Libs.FileHelpers.Helpers
{
    internal sealed class ConditionHelper
    {
        private ConditionHelper()
        {
        }

        public static bool BeginsWith(string line, string selector)
        {
            return line.StartsWith(selector);
        }

        public static bool EndsWith(string line, string selector)
        {
            return line.EndsWith(selector);
        }

        public static bool Contains(string line, string selector)
        {
            return line.IndexOf(selector) >= 0;
        }

        public static bool Enclosed(string line, string selector)
        {
            return line.StartsWith(selector) && line.EndsWith(selector);
        }
    }
}