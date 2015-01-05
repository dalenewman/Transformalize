using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Transformalize.Main.Providers.File;

namespace Transformalize.Configuration {

    public class FileInspectionConfigurationElement : ConfigurationElement {

        private const string NAME = "name";
        private const string TYPES = "types";
        private const string DELIMITERS = "delimiters";
        private const string SAMPLE = "sample";
        private const string MAX_LENGTH = "max-length";
        private const string MIN_LENGTH = "min-length";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[SAMPLE] as string; }
            set { this[SAMPLE] = value; }
        }

        [ConfigurationProperty(SAMPLE, IsRequired = false, DefaultValue = "100")]
        public decimal Sample {
            get { return Convert.ToDecimal(this[SAMPLE]); }
            set { this[SAMPLE] = value; }
        }

        [ConfigurationProperty(MIN_LENGTH, IsRequired = false, DefaultValue = 0)]
        public int MinLength {
            get { return Convert.ToInt32(this[MIN_LENGTH]); }
            set { this[MIN_LENGTH] = value; }
        }

        [ConfigurationProperty(MAX_LENGTH, IsRequired = false, DefaultValue = 0)]
        public int MaxLength {
            get { return Convert.ToInt32(this[MAX_LENGTH]); }
            set { this[MAX_LENGTH] = value; }
        }

        [ConfigurationProperty(TYPES)]
        public TypeElementCollection Types {
            get { return this[TYPES] as TypeElementCollection; }
            set { this[TYPES] = value; }
        }

        [ConfigurationProperty(DELIMITERS)]
        public DelimiterElementCollection Delimiters {
            get { return this[DELIMITERS] as DelimiterElementCollection; }
            set { this[DELIMITERS] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        public FileInspectionRequest GetInspectionRequest() {
            return new FileInspectionRequest() {
                DataTypes = Types.Cast<TypeConfigurationElement>().Select(t => t.Type).ToList(),
                DefaultLength = MinLength == 0 ? "1024" : MinLength.ToString(CultureInfo.InvariantCulture),
                DefaultType = "string",
                MinLength = MinLength,
                MaxLength = MaxLength,
                Delimiters = Delimiters.Cast<DelimiterConfigurationElement>().ToDictionary(d => d.Character[0], d => d.Name),
                Sample = Sample,
                IgnoreEmpty = Types.IgnoreEmpty
            };
        }

    }
}