using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using System;
using System.IO;

namespace Transformalize.Providers.Lucene {
   public abstract class AbstractField : IIndexableField {
      public abstract string Name { get; }
      public abstract IIndexableFieldType IndexableFieldType { get; }
      public abstract float Boost { get; }
      public abstract NumericFieldType NumericType { get; }

      public abstract BytesRef GetBinaryValue();
      public abstract byte? GetByteValue();
      public abstract double? GetDoubleValue();
      public abstract short? GetInt16Value();
      public abstract int? GetInt32Value();
      public abstract long? GetInt64Value();
      public abstract object GetNumericValue();
      public abstract TextReader GetReaderValue();
      public abstract float? GetSingleValue();
      public abstract string GetStringValue();
      public abstract string GetStringValue(IFormatProvider provider);
      public abstract string GetStringValue(string format);
      public abstract string GetStringValue(string format, IFormatProvider provider);
      public abstract TokenStream GetTokenStream(Analyzer analyzer);
   }
}
