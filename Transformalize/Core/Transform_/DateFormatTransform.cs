/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using Transformalize.Core.Parameters_;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_ {
    public class DateFormatTransform : AbstractTransform {
        private readonly string _format;

        public override string Name {
            get { return "Date Format Transform"; }
        }

        public DateFormatTransform(string format, IParameters parameters)
            : base(parameters) {
            _format = format;
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override object Transform(object value)
        {
            return ((DateTime) value).ToString(_format);
        }

        public override void Transform(ref Row row, string resultKey)
        {
            row[resultKey] = ((DateTime) row[FirstParameter.Key]).ToString(_format);
        }

    }
}