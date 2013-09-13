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

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Transformalize.Libs.fastJSON
{
    internal class DynamicJson : DynamicObject
    {
        public DynamicJson(string json)
        {
            var dictionary = JSON.Instance.Parse(json);
            if (dictionary is IDictionary<string, object>)
                Dictionary = (IDictionary<string, object>) dictionary;
        }

        private DynamicJson(object dictionary)
        {
            if (dictionary is IDictionary<string, object>)
                Dictionary = (IDictionary<string, object>) dictionary;
        }

        private IDictionary<string, object> Dictionary { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Dictionary.TryGetValue(binder.Name, out result) == false)
                if (Dictionary.TryGetValue(binder.Name.ToLower(), out result) == false)
                    return false; // throw new Exception("property not found " + binder.Name);

            if (result is IDictionary<string, object>)
                result = new DynamicJson(result as IDictionary<string, object>);

            else if (result is List<object> && (result as List<object>) is IDictionary<string, object>)
                result =
                    new List<DynamicJson>(
                        (result as List<object>).Select(x => new DynamicJson(x as IDictionary<string, object>)));

            else if (result is List<object>)
                result = result as List<object>;

            return Dictionary.ContainsKey(binder.Name);
        }
    }
}