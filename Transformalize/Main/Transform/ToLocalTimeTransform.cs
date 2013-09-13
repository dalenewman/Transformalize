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
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public class ToLocalTimeTransform : AbstractTransform
    {
        public ToLocalTimeTransform(IParameters parameters) : base(parameters)
        {
            Name = "To Local Time";
        }

        public override object Transform(object value)
        {
            return ((DateTime) value).ToLocalTime();
        }

        public override void Transform(ref Row row, string resultKey)
        {
            row[resultKey] = ((DateTime) row[FirstParameter.Key]).ToLocalTime();
        }
    }
}