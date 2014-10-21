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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main {

    public class FieldReader : IFieldReader {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Entity _entity;
        private readonly bool _usePrefix;
        private readonly Process _process;

        public FieldReader(Process process, Entity entity, bool usePrefix = true) {
            _process = process;
            _usePrefix = usePrefix;
            _entity = entity;
        }

        public Field Read(FieldConfigurationElement element, FieldType fieldType = FieldType.NonKey) {
            var alias = Common.GetAlias(element, _usePrefix, _entity.Prefix);

            var field = new Field(element.Type, element.Length, fieldType, element.Output, element.Default) {
                Process = _process.Name,
                Entity = _entity.Alias,
                EntityIndex = _entity.Index,
                EntityOutputName = _entity.OutputName(),
                Index = element.Index,
                Schema = _entity.Schema,
                Name = element.Name,
                Alias = alias,
                Precision = element.Precision,
                Scale = element.Scale,
                Input = element.Input,
                NodeType = element.NodeType,
                ReadInnerXml = element.ReadInnerXml,
                Unicode = element.Unicode.Equals("[default]") ? _entity.Unicode : Convert.ToBoolean(element.Unicode),
                VariableLength = element.VariableLength.Equals("[default]") ? _entity.VariableLength : Convert.ToBoolean(element.VariableLength),
                Aggregate = element.Aggregate.ToLower(),
                Sort = element.Sort.ToLower(),
                Label = element.Label,
                DefaultBlank = element.DefaultBlank,
                DefaultWhiteSpace = element.DefaultWhiteSpace,
                QuotedWith = element.QuotedWith,
                Optional = element.Optional,
                Raw = element.Raw,
                Delimiter = element.Delimiter,
                Distinct = element.Distinct
            };

            if (!field.SimpleType.Equals("string")) {
                field.DefaultBlank = true;
            }

            FieldSearchTypesLoader(field, element);

            foreach (var keyField in new[] { "TflKey", "TflUpdate", "TflBatchId", "TflFileName", "TflDeleted", "TflAction" }) {
                if (field.Alias.Equals(keyField, IC) && field.Input) {
                    TflLogger.Warn(_entity.ProcessName, _entity.Name, "{0}, defined in {1}, is a reserved field name.  Please alias this field.", field.Alias, field.Entity);
                }
            }

            return field;
        }

        private void FieldSearchTypesLoader(Field field, FieldConfigurationElement element) {
            var searchTypes = element.SearchTypes.Cast<FieldSearchTypeConfigurationElement>().ToArray();

            if (searchTypes.Length > 0) {
                foreach (var st in searchTypes.Where(st => _process.SearchTypes.ContainsKey(st.Type))) {
                    field.SearchTypes.Add(InheritType(_process.SearchTypes[st.Type], field));
                }
                return;
            }

            var searchType = element.SearchType.ToLower();
            if (_process.SearchTypes.Any()) {
                if (_process.SearchTypes.ContainsKey(searchType)) {
                    field.SearchTypes.Add(InheritType(_process.SearchTypes[searchType], field));
                }
            }

        }

        private static SearchType InheritType(SearchType searchType, Field field) {
            var newSearchType = new SearchType {
                Name = searchType.Name,
                Index = searchType.Index,
                Store = searchType.Store,
                Type = searchType.Analyzer.Equals(string.Empty) ? field.SimpleType : searchType.Analyzer,
                MultiValued = searchType.MultiValued,
                Analyzer = searchType.Analyzer
            };
            return newSearchType;
        }

    }
}