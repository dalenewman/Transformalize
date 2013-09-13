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
using System.Diagnostics;
using Transformalize.Libs.RazorEngine.Common;

namespace Transformalize.Libs.RazorEngine
{
    [DebuggerDisplay("({Position})\"{Value}\"")]
    public class PositionTagged<T>
    {
        private PositionTagged()
        {
            Position = 0;
            Value = default(T);
        }

        public PositionTagged(T value, int offset)
        {
            Position = offset;
            Value = value;
        }

        public int Position { get; private set; }
        public T Value { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as PositionTagged<T>;
            return other != null &&
                   other.Position == Position &&
                   Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            return HashCodeCombiner.Start()
                                   .Add(Position)
                                   .Add(Value)
                                   .CombinedHash;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator T(PositionTagged<T> value)
        {
            return value.Value;
        }

        public static implicit operator PositionTagged<T>(Tuple<T, int> value)
        {
            return new PositionTagged<T>(value.Item1, value.Item2);
        }

        public static bool operator ==(PositionTagged<T> left, PositionTagged<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PositionTagged<T> left, PositionTagged<T> right)
        {
            return !Equals(left, right);
        }
    }
}