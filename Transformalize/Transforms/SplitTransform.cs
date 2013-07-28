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
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class SplitTransform : AbstractTransform {
        private readonly char[] _separatorArray;
        private readonly int _count;
        private int _index;

        protected override string Name {
            get { return "Split Transform"; }
        }

        public SplitTransform(string separator, IParameters parameters, Dictionary<string, Field> results)
            : base(parameters, results) {
            _separatorArray = separator.ToCharArray();
            _count = Results.Count;
        }

        public override void Transform(ref Row row)
        {
            var split = row[FirstParameter.Key].ToString().Split(_separatorArray, _count);
            _index = 0;
            foreach (var pair in Results) {
                row[pair.Key] = split[_index];
                _index++;
            }
        }
    }
}