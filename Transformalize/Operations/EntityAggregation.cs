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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.FileHelpers.DataLink;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {

    public class EntityAggregation : AbstractAggregationOperation {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly IDictionary<string, ConcurrentDictionary<ObjectArrayKeys, List<object>>> _lists = new Dictionary<string, ConcurrentDictionary<ObjectArrayKeys, List<object>>>();
        private readonly IDictionary<string, Dictionary<ObjectArrayKeys, Dictionary<object, byte>>> _distinct = new Dictionary<string, Dictionary<ObjectArrayKeys, Dictionary<object, byte>>>();
        private readonly Field[] _fieldsToAccumulate;
        private readonly string[] _keysToGroupBy;
        private readonly string _firstKey;
        private readonly Dictionary<string, byte> _needZero = new Dictionary<string, byte>() {
            {"count",0},
            {"sum",0},
            {"maxlength",0}
        };

        public EntityAggregation(Entity entity) {

            _keysToGroupBy = new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Context().ToEnumerable().Where(f => f.Aggregate.Equals("group", IC)).Select(f => f.Alias).ToArray();
            _firstKey = _keysToGroupBy[0];

            _fieldsToAccumulate = new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Context().ToEnumerable().Where(f => !f.Aggregate.Equals("group", IC)).ToArray();

            foreach (var field in _fieldsToAccumulate) {
                _lists[field.Alias] = new ConcurrentDictionary<ObjectArrayKeys, List<object>>();
                if (field.Aggregate.Equals("count", IC) && field.Distinct) {
                    _distinct[field.Alias] = new Dictionary<ObjectArrayKeys, Dictionary<object, byte>>();
                }
            }
        }

        protected override void Accumulate(Row row, Row aggregate) {
            //init
            if (!aggregate.ContainsKey(_firstKey)) {
                foreach (var alias in _keysToGroupBy) {
                    aggregate[alias] = row[alias];
                }

                foreach (var field in _fieldsToAccumulate) {
                    if (_needZero.ContainsKey(field.Aggregate)) {
                        aggregate[field.Alias] = 0;
                    } else {
                        if (field.Aggregate.Equals("minlength")) {
                            aggregate[field.Alias] = row[field.Alias].ToString().Length;
                        } else {
                            aggregate[field.Alias] = row[field.Alias];
                        }
                    }
                }
            }

            //accumulate
            foreach (var field in _fieldsToAccumulate)
            {
                int len;
                switch (field.Aggregate) {
                    case "count":
                        if (field.Distinct) {
                            var key = row.CreateKey(_keysToGroupBy);
                            var value = row[field.Name];
                            if (!_distinct[field.Alias].ContainsKey(key)) {
                                _distinct[field.Alias].Add(key, new Dictionary<object, byte>());
                            }
                            if (!_distinct[field.Alias][key].ContainsKey(value)) {
                                _distinct[field.Alias][key].Add(value, 0);
                                aggregate[field.Alias] = (int)aggregate[field.Alias] + 1;
                            }
                            break;
                        }
                        aggregate[field.Alias] = (int)aggregate[field.Alias] + 1;
                        break;
                    case "sum":
                        //switch (field.SimpleType) {
                        //    case "int32":
                        //        aggregate[field.Alias] = (int)aggregate[field.Alias] + (int)row[field.Alias];
                        //        break;
                        //}
                        aggregate[field.Alias] = (dynamic)aggregate[field.Alias] + (dynamic)row[field.Alias];
                        break;
                    case "max":
                        switch (field.SimpleType) {
                            case "byte[]":
                                aggregate[field.Alias] = Max((byte[])aggregate[field.Alias], (byte[])row[field.Alias]);
                                break;
                            default:
                                var comparable = aggregate[field.Alias] as IComparable;
                                if (comparable != null) {
                                    if (comparable.CompareTo(row[field.Alias]) < 0) {
                                        aggregate[field.Alias] = row[field.Alias];
                                    }
                                }
                                break;
                        }
                        break;
                    case "min":
                        switch (field.SimpleType) {
                            case "byte[]":
                                aggregate[field.Alias] = Min((byte[])aggregate[field.Alias], (byte[])row[field.Alias]);
                                break;
                            default:
                                var comparable = aggregate[field.Alias] as IComparable;
                                if (comparable != null) {
                                    if (comparable.CompareTo(row[field.Alias]) > 0) {
                                        aggregate[field.Alias] = row[field.Alias];
                                    }
                                }
                                break;
                        }
                        break;

                    case "maxlength":
                        len = row[field.Alias].ToString().Length;
                        if (len > (dynamic)aggregate[field.Alias]) {
                            aggregate[field.Alias] = len;
                        }
                        break;

                    case "minlength":
                        len = row[field.Alias].ToString().Length;
                        if (len < (dynamic)aggregate[field.Alias]) {
                            aggregate[field.Alias] = len;
                        }
                        break;

                    case "last":
                        aggregate[field.Alias] = row[field.Alias];
                        break;

                    case "join":
                        Keep(field.Alias, row);
                        break;

                    case "array":
                        Keep(field.Alias, row);
                        break;
                }
            }
        }

        protected override string[] GetColumnsToGroupBy() {
            return _keysToGroupBy;
        }

        private void Keep(string alias, Row row) {
            var key = row.CreateKey(_keysToGroupBy);
            if (_lists[alias].ContainsKey(key)) {
                _lists[alias][key].Add(row[alias]);
            } else {
                _lists[alias][key] = new List<object>() { row[alias] };
            }
        }

        public static byte[] Max(byte[] b1, byte[] b2) {
            var minLength = Math.Min(b1.Length, b2.Length);
            if (minLength == 0)  // return longest, when comparable are equal
            {
                return b1.Length > b2.Length ? b1 : b2;
            }

            for (var i = 0; i < minLength; i++) {
                if (b1[i] != b2[i]) {
                    return b1[i] > b2[i] ? b1 : b2;  // return first one with a bigger byte
                }
            }

            return b1.Length > b2.Length ? b1 : b2; // return longest, when comparable are equal
        }

        public static byte[] Min(byte[] b1, byte[] b2) {
            var minLength = Math.Min(b1.Length, b2.Length);
            if (minLength == 0)  // return shortest, when comparable are equal
            {
                return b1.Length < b2.Length ? b1 : b2;
            }

            for (var i = 0; i < minLength; i++) {
                if (b1[i] != b2[i]) {
                    return b1[i] < b2[i] ? b1 : b2;  // return first one with a smaller byte
                }
            }

            return b1.Length < b2.Length ? b1 : b2; // return smallest, when comparable are equal
        }

        protected override void FinishAggregation(Row aggregate) {
            //final accumulate
            var group = aggregate.CreateKey(_keysToGroupBy);
            foreach (var field in _fieldsToAccumulate) {
                switch (field.Aggregate) {
                    case "join":
                        aggregate[field.Alias] = string.Join(field.Delimiter, field.Distinct ? _lists[field.Alias][@group].Distinct() : _lists[field.Alias][@group]);
                        break;
                    case "array":
                        aggregate[field.Alias] = (field.Distinct ? _lists[field.Alias][@group].Distinct() : _lists[field.Alias][@group]).ToArray();
                        break;
                }
            }
        }
    }
}