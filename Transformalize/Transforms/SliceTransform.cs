#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transforms {

    public class SliceTransform : StringTransform {
        private readonly Field _input;
        private readonly char[] _separator;
        private readonly int? _start;
        private readonly int? _end;
        private readonly int? _step;

        public SliceTransform(IContext context) : base(context, "string") {

            _input = SingleInput();
            _separator = context.Transform.Separator.ToCharArray();

            var split = context.Transform.Expression.Split(':');
            switch (split.Length) {
                case 0:
                    _start = 0;
                    _end = null;
                    _step = 1;
                    break;
                case 1:
                    _start = split[0].IsNumeric() ? new int?(Convert.ToInt32(split[0])) : null;
                    _end = null;
                    _step = 1;
                    break;
                case 2:
                    _start = split[0].IsNumeric() ? new int?(Convert.ToInt32(split[0])) : null;
                    _end = split[1].IsNumeric() ? new int?(Convert.ToInt32(split[1])) : null;
                    _step = 1;
                    break;
                default:
                    _start = split[0].IsNumeric() ? new int?(Convert.ToInt32(split[0])) : null;
                    _end = split[1].IsNumeric() ? new int?(Convert.ToInt32(split[1])) : null;
                    _step = split[2].IsNumeric() ? new int?(Convert.ToInt32(split[2])) : 1;
                    break;
            }
        }

        public override IRow Transform(IRow row) {

            var value = GetString(row, _input);

            if (Context.Transform.Separator == string.Empty) {
                row[Context.Field] = string.Concat(Slice(value.ToCharArray(), _start, _end, _step));
            } else {
                var split = value.Split(_separator);
                row[Context.Field] = string.Join(Context.Transform.Separator, Slice(split, _start, _end, _step));
            }
            Increment();
            return row;
        }

        private static int AdjustEndPoint(int length, int endPoint, int step) {
            if (endPoint < 0) {
                endPoint += length;
                if (endPoint < 0) {
                    endPoint = step < 0 ? -1 : 0;
                }
            } else if (endPoint >= length) {
                endPoint = step < 0 ? length - 1 : length;
            }
            return endPoint;
        }

        private class Slicer {
            public Slicer(int start, int stop, int step) {
                Start = start;
                Stop = stop;
                Step = step;
            }
            public int Start { get; }
            public int Stop { get; }
            public int Step { get; }
        }

        private static Slicer AdjustSlice(int length, int? start, int? stop, int? step) {
            if (step == null) {
                step = 1;
            }

            if (start == null) {
                start = step < 0 ? length - 1 : 0;
            } else {
                start = AdjustEndPoint(length, start.Value, step.Value);
            }

            if (stop == null) {
                stop = step < 0 ? -1 : length;
            } else {
                stop = AdjustEndPoint(length, stop.Value, step.Value);
            }

            return new Slicer(start.Value, stop.Value, step.Value);
        }

        private static IEnumerable<int> SliceIndices(int length, int? start, int? stop, int? step) {
            var slice = AdjustSlice(length, start, stop, step);
            var i = slice.Start;
            if (slice.Step < 0) {
                while (i > slice.Stop) {
                    yield return i;
                    i += slice.Step;
                }
            } else {
                while (i < slice.Stop) {
                    yield return i;
                    i += slice.Step;
                }
            }
        }

        public static IEnumerable<T> Slice<T>(T[] list, int? start = null, int? stop = null, int? step = null) {
            foreach (var index in SliceIndices(list.Length, start, stop, step)) {
                yield return list[index];
            }
        }

    }
}