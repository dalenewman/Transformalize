#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado.Ext {

   class ExpressionContinuation {
      public string Expression { get; set; }
      public string Continuation { get; set; }
   }
   public static class SqlFilterExtensions {

      public static char TextQualifier = '\'';
      public static HashSet<string> ListOperators = new HashSet<string>() { "IN", "NOT IN" };

      public static string ResolveFilter(this IContext c, IConnectionFactory factory) {

         var builder = new StringBuilder();
         var filters = new List<ExpressionContinuation>();

         for (var index = 0; index < c.Entity.Filter.Count; index++) {
            var filter = c.Entity.Filter[index];
            if (filter.Value == filter.WildCard) {
               continue;  // ignore this filter
            } else {
               var tried = new ExpressionContinuation { Expression = ResolveExpression(c, filter, index, factory), Continuation = filter.Continuation };
               if (tried.Expression != string.Empty) {
                  filters.Add(tried);
               }
            }
         }

         var last = filters.Count - 1;

         for (var i = 0; i < filters.Count; i++) {
            var filter = filters[i];

            builder.Append(filter.Expression);

            if (i >= last) {
               continue;
            }

            builder.Append(" ");
            builder.Append(filter.Continuation);
            builder.Append(" ");
         }

         var result = builder.ToString().Trim(' ');

         return result == string.Empty ? string.Empty : $"({builder})";
      }

      private static string ResolveExpression(IContext c, Filter filter, int index, IConnectionFactory factory) {
         if (!string.IsNullOrEmpty(filter.Expression))
            return filter.Expression;

         if (filter.Type == "search" && filter.LeftField != null) {
            var searchType = c.Process.SearchTypes.FirstOrDefault(st => st.Name == filter.LeftField.SearchType);
            if (searchType != null && searchType.Name != "default") {
               return ResolveFullTextExpression(c, filter, index, factory, searchType);
            }
         }

         var resolvedOperator = ResolveOperator(c, filter);
         return $"{ResolveSide(filter, "left", resolvedOperator, index, factory)} {resolvedOperator} {ResolveSide(filter, "right", resolvedOperator, index, factory)}";
      }

      // CONTAINS operator keywords, longest match first so "AND NOT" wins over "AND"
      private static readonly string[] _containsOperators = { "AND NOT", "AND", "OR", "NEAR", "NOT" };

      private enum ContainsTokenKind { Word, Operator, Quoted }

      private static List<(string Text, ContainsTokenKind Kind)> ParseContainsTokens(string value) {
         var result = new List<(string, ContainsTokenKind)>();
         var i = 0;
         while (i < value.Length) {
            while (i < value.Length && char.IsWhiteSpace(value[i])) i++;
            if (i >= value.Length) break;

            if (value[i] == '"') {
               var end = value.IndexOf('"', i + 1);
               if (end == -1) end = value.Length - 1;
               result.Add((value.Substring(i, end - i + 1), ContainsTokenKind.Quoted));
               i = end + 1;
               continue;
            }

            var matched = false;
            foreach (var op in _containsOperators) {
               if (i + op.Length <= value.Length &&
                   string.Compare(value, i, op, 0, op.Length, StringComparison.OrdinalIgnoreCase) == 0 &&
                   (i + op.Length >= value.Length || char.IsWhiteSpace(value[i + op.Length]))) {
                  result.Add((op.ToUpperInvariant(), ContainsTokenKind.Operator));
                  i += op.Length;
                  matched = true;
                  break;
               }
            }
            if (matched) continue;

            var end2 = i;
            while (end2 < value.Length && !char.IsWhiteSpace(value[end2]) && value[end2] != '"') end2++;
            result.Add((value.Substring(i, end2 - i), ContainsTokenKind.Word));
            i = end2;
         }
         return result;
      }

      // Strip invalid leading wildcards and quote valid trailing-wildcard (prefix) tokens.
      // Returns null when the token normalizes to empty and should be dropped.
      private static string NormalizeBareWord(string word) {
         var stripped = word.TrimStart('*');
         if (stripped.Length == 0) return null;
         return stripped.EndsWith("*") ? $"\"{stripped}\"" : stripped;
      }

      private static string NormalizeContainsQuery(string value) {
         var tokens = ParseContainsTokens(value.Trim());
         var parts = new List<string>(tokens.Count * 2);
         var prevWasValue = false;

         foreach (var (text, kind) in tokens) {
            switch (kind) {
               case ContainsTokenKind.Operator:
                  // Only emit an operator when there is a left-hand term.
                  // Dangling leading operators (e.g. "OR chai", "AND something") are skipped.
                  // Consecutive operators (e.g. "chai AND OR chang") keep only the first.
                  if (prevWasValue) {
                     parts.Add(text == "NOT" ? "AND NOT" : text);
                     prevWasValue = false;
                  }
                  break;
               case ContainsTokenKind.Quoted:
                  if (prevWasValue) parts.Add("AND");
                  parts.Add(text);
                  prevWasValue = true;
                  break;
               default: // Word
                  var normalized = NormalizeBareWord(text);
                  if (normalized != null) {
                     if (prevWasValue) parts.Add("AND");
                     parts.Add(normalized);
                     prevWasValue = true;
                  }
                  break;
            }
         }

         // Remove any trailing operator that has no right-hand term (e.g. "chai AND", "chai OR")
         while (parts.Count > 0) {
            var last = parts[parts.Count - 1];
            if (last == "AND" || last == "OR" || last == "AND NOT" || last == "NEAR")
               parts.RemoveAt(parts.Count - 1);
            else
               break;
         }

         return string.Join(" ", parts);
      }

      private static string ResolveFullTextExpression(IContext c, Filter filter, int index, IConnectionFactory factory, SearchType searchType) {
         var fieldName = factory.Enclose(filter.Field);
         var parameter = ParameterName(index);
         var negate = ConvertOperator(filter.Operator) == "!=";

         string expr;
         switch (factory.AdoProvider) {
            case AdoProvider.SqlServer:
               var langClause = string.IsNullOrEmpty(searchType.Analyzer) ? string.Empty : $" LANGUAGE '{searchType.Analyzer}'";
               if (searchType.QueryType == "freetext") {
                  expr = $"FREETEXT({fieldName}, {parameter}{langClause})";
               } else {
                  expr = $"CONTAINS({fieldName}, {parameter}{langClause})";
               }
               break;
            case AdoProvider.PostgreSql:
               var lang = string.IsNullOrEmpty(searchType.Analyzer) ? "english" : searchType.Analyzer;
               var tsQueryFn = searchType.QueryType switch {
                  "web" => "websearch_to_tsquery",
                  "phrase" => "phraseto_tsquery",
                  "raw" => "to_tsquery",
                  _ => "plainto_tsquery"
               };
               expr = $"to_tsvector('{lang}', {fieldName}) @@ {tsQueryFn}('{lang}', {parameter})";
               break;
            case AdoProvider.MySql:
               var modeClause = searchType.Mode switch {
                  "natural" => "IN NATURAL LANGUAGE MODE",
                  "expansion" => "WITH QUERY EXPANSION",
                  _ => "IN BOOLEAN MODE"
               };
               expr = $"MATCH({fieldName}) AGAINST({parameter} {modeClause})";
               break;
            case AdoProvider.SqLite:
               var ftsTable = factory.Enclose(c.Entity.Name + "_fts");
               expr = $"rowid IN (SELECT rowid FROM {ftsTable} WHERE {ftsTable} MATCH {parameter})";
               break;
            default:
               expr = $"{fieldName} LIKE {parameter}";
               break;
         }

         return negate ? $"NOT ({expr})" : expr;
      }

      private static string ResolveSide(Filter filter, string side, string resolvedOperator, int index, IConnectionFactory factory) {

         bool isField;
         string value;
         bool otherIsField;
         Field otherField;

         if (side == "left") {
            isField = filter.IsField;
            value = filter.Field;
            otherIsField = filter.ValueIsField;
            otherField = filter.ValueField;
         } else {
            isField = filter.ValueIsField;
            value = filter.Value;
            otherIsField = filter.IsField;
            otherField = filter.LeftField;
         }

         if (isField)
            return factory.Enclose(value);

         if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
            return "NULL";

         if (!otherIsField) {
            return ParameterName(index, side);
         }

         if (ListOperators.Contains(resolvedOperator)) {
            var items = new List<string>();
            var itemIndex = 0;
            foreach (var item in value.Split(filter.Delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
               items.Add(ParameterName(index, side, itemIndex++));
            }
            return "(" + string.Join(",", items) + ")";
         } else {
            return ParameterName(index, side);
         }
      }

      /// <summary>
      /// Adds both parameters referenced by a user-supplied query and the generated parameters used by entity filters.
      /// </summary>
      public static void AddAdoParameters(this IDbCommand command, IContext context, IConnectionFactory factory) {
         var referenced = new HashSet<string>(new AdoParameterFinder().Find(command.CommandText), StringComparer.OrdinalIgnoreCase);
         if (referenced.Count == 0) {
            return;
         }

         var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

         foreach (IDataParameter existing in command.Parameters) {
            added.Add(existing.ParameterName.TrimStart('@'));
         }

         for (var index = 0; index < context.Entity.Filter.Count; index++) {
            var filter = context.Entity.Filter[index];
            if (filter.Value == filter.WildCard || !string.IsNullOrEmpty(filter.Expression)) {
               continue;
            }

            if (filter.Type == "search" && filter.LeftField != null) {
               var searchType = context.Process.SearchTypes.FirstOrDefault(st => st.Name == filter.LeftField.SearchType);
               if (searchType != null && searchType.Name != "default") {
                  var value = factory.AdoProvider == AdoProvider.SqlServer && searchType.QueryType != "freetext"
                     ? NormalizeContainsQuery(filter.Value)
                     : filter.Value;
                  AddFilterParameter(command, referenced, added, ParameterName(index), value);
                  continue;
               }
            }

            var resolvedOperator = ResolveOperator(context, filter);
            AddSideParameters(command, referenced, added, filter, "left", resolvedOperator, index);
            AddSideParameters(command, referenced, added, filter, "right", resolvedOperator, index);
         }

         foreach (var parameter in context.Process.Parameters) {
            if (referenced.Contains(parameter.Name) && added.Add(parameter.Name)) {
               AddParameter(command, parameter.Name, parameter.Convert(parameter.Value));
            }
         }
      }

      private static void AddSideParameters(IDbCommand command, HashSet<string> referenced, HashSet<string> added, Filter filter, string side, string resolvedOperator, int index) {
         var isLeft = side == "left";
         var isField = isLeft ? filter.IsField : filter.ValueIsField;
         var value = isLeft ? filter.Field : filter.Value;
         var otherIsField = isLeft ? filter.ValueIsField : filter.IsField;
         var otherField = isLeft ? filter.ValueField : filter.LeftField;

         if (isField || value.Equals("null", StringComparison.OrdinalIgnoreCase)) {
            return;
         }

         if (otherIsField && ListOperators.Contains(resolvedOperator)) {
            var itemIndex = 0;
            foreach (var item in value.Split(filter.Delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
               AddFilterParameter(command, referenced, added, ParameterName(index, side, itemIndex++), ConvertFilterValue(otherField, item));
            }
            return;
         }

         if (otherIsField && filter.Type == "search" && filter.WildCard != "%") {
            value = value.Contains(filter.WildCard) ? value.Replace(filter.WildCard, "%") : $"%{value}%";
         }

         AddFilterParameter(command, referenced, added, ParameterName(index, side), otherIsField ? ConvertFilterValue(otherField, value) : value);
      }

      private static object ConvertFilterValue(Field field, string value) {
         return field == null || AdoConstants.StringTypes.Contains(field.Type) ? value : field.Convert(value);
      }

      private static void AddFilterParameter(IDbCommand command, HashSet<string> referenced, HashSet<string> added, string name, object value) {
         var bareName = name.TrimStart('@');
         if (referenced.Contains(bareName) && added.Add(bareName)) {
            AddParameter(command, name, value);
         }
      }

      private static void AddParameter(IDbCommand command, string name, object value) {
         var parameter = command.CreateParameter();
         parameter.ParameterName = name;
         parameter.Direction = ParameterDirection.Input;
         parameter.Value = value ?? DBNull.Value;
         command.Parameters.Add(parameter);
      }

      private static string ParameterName(int index, string side = "right", int? itemIndex = null) {
         return $"@TflFilter{index}{(side == "left" ? "Left" : string.Empty)}{(itemIndex.HasValue ? "_" + itemIndex.Value : string.Empty)}";
      }

      private static string ResolveOperator(IContext context, Filter filter) {
         var converted = ConvertOperator(filter.Operator);
         switch (filter.Type) {
            case "search":
               if (converted == "=") {
                  return "LIKE";
               }
               if (converted == "!=") {
                  return "NOT LIKE";
               }
               return converted;
            case "facet":
               var parameter = context.Process.Parameters.FirstOrDefault(p => p.Map == filter.Map);
               if(parameter != null && parameter.Multiple) {
                  if(converted == "=") {
                     return "IN";
                  }
                  if(converted == "!=") {
                     return "NOT IN";
                  }
               }
               return converted;
            default:
               return converted;
         }
      }

      private static string ConvertOperator(string op) {
         switch (op) {
            case "gt":
            case "greaterthan":
               return ">";
            case "gte":
            case "greaterthanequal":
               return ">=";
            case "lt":
            case "lessthan":
               return "<";
            case "lte":
            case "lessthanequal":
               return "<=";
            case "!=":
            case "!==":
            case "notequal":
            case "notequals":
               return "!=";
            case "in":
               return "IN";
            case "notin":
               return "NOT IN";
            case "like":
               return "LIKE";
            case "notlike":
               return "NOT LIKE";
            default:
               return "=";
         }

      }

      public static string ResolveOrder(this InputContext context, IConnectionFactory cf) {
         if (!context.Entity.Order.Any())
            return string.Empty;
         var orderBy = string.Join(", ", context.Entity.Order.Select(o => $"{cf.Enclose(o.Field)} {o.Sort.ToUpper()}"));
         context.Debug(() => $"ORDER: {orderBy}");
         return $" ORDER BY {orderBy}";
      }
   }
}
