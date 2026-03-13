using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Configuration;

namespace Transformalize.Providers.Lucene {
   public class FieldSearchType {
      public Field Field { get; set; }
      public SearchType SearchType { get; set; }
   }

   public class FieldSearchTypes : IEnumerable<FieldSearchType> {
      private readonly List<FieldSearchType> _fields = new List<FieldSearchType>();
      public FieldSearchTypes(Process process, IEnumerable<Field> fields) {
         foreach(var field in fields) {
            var fst = new FieldSearchType {
               Field = field,
               SearchType = process.SearchTypes.FirstOrDefault(st => st.Name == field.SearchType)
            };
            _fields.Add(fst);
         }
      }

      public IEnumerator<FieldSearchType> GetEnumerator() {
         return ((IEnumerable<FieldSearchType>)_fields).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return ((IEnumerable)_fields).GetEnumerator();
      }
   }
}
