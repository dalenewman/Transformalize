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

using System.Collections;
using System.Collections.Generic;

namespace Transformalize.Main
{
    public class Parameters : IParameters, IEnumerable<KeyValuePair<string, IParameter>>
    {
        private readonly ConversionFactory _conversionFactory = new ConversionFactory();
        private readonly IDictionary<string, IParameter> _items = new Dictionary<string, IParameter>();
        private KeyValuePair<string, IParameter> _first;

        IEnumerator<KeyValuePair<string, IParameter>> IEnumerable<KeyValuePair<string, IParameter>>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public IEnumerable<string> Keys
        {
            get { return _items.Keys; }
        }

        public IEnumerator<KeyValuePair<string, IParameter>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IParameter this[string key]
        {
            get { return _items[key]; }
            set
            {
                _items[key] = value;
                RecordFirst(key);
            }
        }

        public void Add(string field, string name, object value, string type)
        {
            var parameter = new Parameter
                                {
                                    Name = name,
                                    Value = value == null ? null : _conversionFactory.Convert(value, type),
                                    SimpleType = Common.ToSimpleType(type)
                                };
            Add(field, parameter);
        }

        public KeyValuePair<string, IParameter> First()
        {
            return _first;
        }

        public bool Any()
        {
            return _first.Key != null;
        }

        public void Add(string field, IParameter parameter)
        {
            _items.Add(field, parameter);
            RecordFirst(field, parameter);
        }

        private void RecordFirst(string field, IParameter parameter)
        {
            if (_first.Key == null)
            {
                _first = new KeyValuePair<string, IParameter>(field, parameter);
            }
        }

        private void RecordFirst(string key)
        {
            if (_first.Key == null)
            {
                _first = new KeyValuePair<string, IParameter>(key, _items[key]);
                ;
            }
        }

        public void AddRange(IParameters parameters)
        {
            foreach (var parameter in parameters)
            {
                Add(parameter.Key, parameter.Value);
            }
        }
    }
}