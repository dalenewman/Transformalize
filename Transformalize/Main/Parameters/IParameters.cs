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
using Transformalize.Main.Parameters;

namespace Transformalize.Main {
    public interface IParameters {
        int Count { get; }
        IEnumerable<string> Keys { get; }
        IParameter this[string key] { get; set; }
        IParameter this[int index] { get; }
        IEnumerator<KeyValuePair<string, IParameter>> GetEnumerator();
        void Add(string field, string name, object value, string type);
        KeyValuePair<string, IParameter> First();
        bool Any();
        IEnumerable<KeyValuePair<string, IParameter>> ToEnumerable();
        bool ContainsKey(string key);
        void Remove(string key);
        ExpandoObject ToExpandoObject();
    }
}