using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Data
{
    public class ConfigurationMapReader : IMapReader {
        private readonly ItemElementCollection _items;
        private readonly string _operator;

        public ConfigurationMapReader(ItemElementCollection items, string @operator)
        {
            _items = items;
            _operator = @operator;
        }

        public Dictionary<string, object> Read() {
            var map = new Dictionary<string, object>();
            foreach (var i in _items.Cast<ItemConfigurationElement>().Where(i => i.Operator.Equals(_operator, StringComparison.OrdinalIgnoreCase))) {
                map[i.From] = i.To;
            }
            return map;
        }
    }
}