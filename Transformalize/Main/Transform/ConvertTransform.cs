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
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public class ConvertTransform : AbstractTransform
    {
        private readonly Dictionary<string, Func<object, object>> _conversionMap;

        private readonly Dictionary<string, Func<object, object>> _specialConversionMap = new Dictionary
            <string, Func<object, object>>
                                                                                              {
                                                                                                  {
                                                                                                      "int32",
                                                                                                      (x =>
                                                                                                       Common
                                                                                                           .DateTimeToInt32
                                                                                                           ((DateTime) x))
                                                                                                  },
                                                                                              };

        private readonly string _to;

        public ConvertTransform(string to, IParameters parameters)
            : base(parameters)
        {
            Name = "Convert";
            _to = Common.ToSimpleType(to);

            if (HasParameters && FirstParameter.Value.SimpleType == "datetime" && _to == "int32")
                _conversionMap = _specialConversionMap;
            else
                _conversionMap = Common.ObjectConversionMap;
        }

        public override object Transform(object value)
        {
            return Common.ObjectConversionMap[_to](value);
        }

        public override void Transform(ref Row row, string resultKey)
        {
            row[resultKey] = _conversionMap[_to](row[FirstParameter.Key]);
        }
    }
}