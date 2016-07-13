using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Ext;
using Orchard;
using Pipeline.Configuration;

namespace Pipeline.Web.Orchard.Services {

    public interface ISortService : IDependency {
        SortDirection Sort(int fieldNumber, string expression);
        void AddSortToEntity(Entity entity, string expression);
    }

    public class SortService : ISortService {

        private readonly Dictionary<int, char> _cache = null;

        private static Dictionary<int, char> ProcessExpression(string expression) {
            var order = expression ?? string.Empty;
            var orderLookup = order.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            var dict = new Dictionary<int, char>();
            foreach (var item in orderLookup) {
                var direction = item.EndsWith("d") ? 'd' : 'a';
                var value = item.TrimEnd('a', 'd');
                int number;
                if (int.TryParse(value, out number)) {
                    dict[number] = direction;
                }
            }
            return dict;
        }

        public SortDirection Sort(int fieldNumber, string expression) {
            var lookup = _cache ?? ProcessExpression(expression);

            if (lookup.ContainsKey(fieldNumber)) {
                return lookup[fieldNumber] == 'a' ? SortDirection.Asc : SortDirection.Desc;
            }

            return SortDirection.None;
        }

        public void AddSortToEntity(Entity entity, string expression) {
            string orderBy = null;
            var fields = entity.GetAllOutputFields().ToArray();
            for (var i = 0; i < fields.Length; i++) {
                var field = fields[i];
                if (!field.Sortable) {
                    continue;
                }
                var number = i + 1;
                var sort = Sort(number, expression);
                if (sort != SortDirection.None) {
                    if (string.IsNullOrEmpty(entity.Query)) {
                        entity.Order.Add(new Order { Field = field.SortField, Sort = sort == SortDirection.Asc ? "asc" : "desc" }.WithDefaults());
                    } else {
                        if (orderBy == null) {
                            entity.Query = entity.Query.TrimEnd(';');
                            orderBy = " ORDER BY ";
                        }
                        orderBy += " [" + field.SortField + "] " + (sort == SortDirection.Asc ? "ASC" : "DESC") + ",";
                    }
                }
            }

            if (!string.IsNullOrEmpty(orderBy)) {
                entity.Query += orderBy.TrimEnd(',');
            }

        }
    }
}