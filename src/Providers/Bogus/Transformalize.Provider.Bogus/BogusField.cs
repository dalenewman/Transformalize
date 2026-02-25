using System;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Bogus {
   public class BogusField : IField {
      private readonly Field _field;
      public BogusField(Field field) {
         _field = field;
      }

      public int Length => _field.Length == "max" ? int.MaxValue : int.Parse(_field.Length);
      public string Format => _field.Format;
      public string Name => _field.Name;
      public string Alias => _field.Alias;
      public short Index => _field.Index;
      public short MasterIndex => _field.MasterIndex;
      public short KeyIndex => _field.KeyIndex;
      public string Type => _field.Type;
      public string Map => _field.Map;
      public object Max { get; set; }
      public int MaxInt { get; set; }
      public double MaxDouble { get; set; }
      public decimal MaxDecimal { get; set; }
      public bool HasMax { get; set; }
      public object Min { get; set; }
      public int MinInt { get; set; }
      public double MinDouble { get; set; }
      public decimal MinDecimal { get; set; }
      public bool HasMin { get; set; }
      public bool HasMinAndMax { get; set; }
      public byte MaxByte { get; set; }
      public byte MinByte { get; set; }
      public bool HasMap { get; set; }
      public MapItem[] MapItems { get; set; }
      public string Hint => _field.Hint == string.Empty ? "product" : _field.Hint;
      public DateTime MaxDateTime { get; set; }
      public DateTime MinDateTime { get; set; }
      public string Delimiter => _field.Delimiter;
   }
}