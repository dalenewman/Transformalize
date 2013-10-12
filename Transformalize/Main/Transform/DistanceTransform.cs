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
using System.Device.Location;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main {

    public class DistanceTransform : AbstractTransform {
        private readonly string _units;
        private readonly int _paramCount;
        private readonly SortedDictionary<string, Tuple<string, object>> _params = new SortedDictionary<string, Tuple<string, object>>();

        public DistanceTransform(string units, IParameters parameters)
            : base(parameters) {
            _units = units;
            Name = "Distance";
            RequiresRow = true;
            RequiresParameters = true;
            _paramCount = Parameters.Count;
            if (_paramCount < 4) {
                Log.Error("You have a distance transform with less than 4 parameters.  It needs fromLat, fromLong, toLat, toLong.");
                Environment.Exit(1);
            }
            _params["fromLat"] = new Tuple<string, object>(Parameters[0].Name, Parameters[0].Value);
            _params["fromLong"] = new Tuple<string, object>(Parameters[1].Name, Parameters[1].Value);
            _params["toLat"] = new Tuple<string, object>(Parameters[2].Name, Parameters[2].Value);
            _params["toLong"] = new Tuple<string, object>(Parameters[3].Name, Parameters[3].Value);
        }

        public override void Transform(ref Row row, string resultKey) {

            var fromLat = Convert.ToDouble(row[_params["fromLat"].Item1] ?? _params["fromLat"].Item2);
            var fromLong = Convert.ToDouble(row[_params["fromLong"].Item1] ?? _params["fromLong"].Item2);
            var toLat = Convert.ToDouble(row[_params["toLat"].Item1] ?? _params["toLat"].Item2);
            var toLong = Convert.ToDouble(row[_params["toLong"].Item1] ?? _params["toLong"].Item2);

            var meters = new GeoCoordinate(fromLat, fromLong).GetDistanceTo(new GeoCoordinate(toLat, toLong));
            switch (_units) {
                case "miles":
                    row[resultKey] = 0.000621371 * meters;
                    break;
                case "kilometers":
                    row[resultKey] = 0.001 * meters;
                    break;
                default:
                    row[resultKey] = meters;
                    break;
            };
        }
    }
}