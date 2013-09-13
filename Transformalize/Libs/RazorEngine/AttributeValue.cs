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

namespace Transformalize.Libs.RazorEngine
{
    public class AttributeValue
    {
        public AttributeValue(PositionTagged<string> prefix, PositionTagged<object> value, bool literal)
        {
            Prefix = prefix;
            Value = value;
            Literal = literal;
        }

        public PositionTagged<string> Prefix { get; private set; }
        public PositionTagged<object> Value { get; private set; }
        public bool Literal { get; private set; }

        public static AttributeValue FromTuple(Tuple<Tuple<string, int>, Tuple<object, int>, bool> value)
        {
            return new AttributeValue(value.Item1, value.Item2, value.Item3);
        }

        public static AttributeValue FromTuple(Tuple<Tuple<string, int>, Tuple<string, int>, bool> value)
        {
            return new AttributeValue(value.Item1, new PositionTagged<object>(value.Item2.Item1, value.Item2.Item2), value.Item3);
        }

        public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<object, int>, bool> value)
        {
            return FromTuple(value);
        }

        public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<string, int>, bool> value)
        {
            return FromTuple(value);
        }
    }
}