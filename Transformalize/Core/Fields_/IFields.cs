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
using System.Collections.Generic;
using System.Text;
using Transformalize.Core.Field_;

namespace Transformalize.Core.Fields_
{
    public interface IFields {
        
        int Count { get; }
        IEnumerable<string> Keys { get; }
        IEnumerator<KeyValuePair<string, Field>> GetEnumerator();
        Field this[string key] { get; set; }
        void Add(string key, Field field);
        void AddRange(IFields fields);
        KeyValuePair<string, Field> First();
        KeyValuePair<string, Field> First(Func<KeyValuePair<string, Field>, bool> predicate);
        bool Any();
        bool Any(Func<KeyValuePair<string, Field>, bool> predicate);
        bool ContainsKey(string key);
        void Remove(string key);
        KeyValuePair<string, Field> Last();
        KeyValuePair<string, Field> Last(Func<KeyValuePair<string, Field>, bool> predicate);
        IEnumerable<Field> ToEnumerable();
    }
}