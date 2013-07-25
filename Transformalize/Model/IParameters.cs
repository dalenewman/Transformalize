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

namespace Transformalize.Model
{
    public interface IParameters {
        int Count { get; }
        IEnumerable<string> Keys { get; }
        IEnumerator<KeyValuePair<string, IParameter>> GetEnumerator();
        IParameter this[string key] { get; set; }
        void Add(string field, string name, object value, string type);
        void Add(string field, IParameter parameter);
        KeyValuePair<string, IParameter> First();
        bool Any();
    }
}