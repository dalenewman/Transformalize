using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main.Providers.File;

namespace Transformalize.Configuration {
    public class TflFileInspection : CfgNode {

        [Cfg(value = (short)0 )]
        public short MaxLength { get; set; }
        [Cfg(value = (short)0)]
        public short MinLength { get; set; }
        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(value = (short)100)]
        public short Sample { get; set; }
        [Cfg(value=false)]
        public bool IgnoreEmpty { get; set; }

        [Cfg(required = false)]
        public List<TflType> Types { get; set; }
        [Cfg(required = true)]
        public List<TflDelimiter> Delimiters { get; set; }


        public FileInspectionRequest GetInspectionRequest() {
            return new FileInspectionRequest() {
                DataTypes = Types.Cast<TypeConfigurationElement>().Select(t => t.Type).ToList(),
                DefaultLength = MinLength == 0 ? "1024" : MinLength.ToString(CultureInfo.InvariantCulture),
                DefaultType = "string",
                MinLength = MinLength,
                MaxLength = MaxLength,
                Delimiters = Delimiters.Cast<DelimiterConfigurationElement>().ToDictionary(d => d.Character[0], d => d.Name),
                Sample = Sample,
                IgnoreEmpty = IgnoreEmpty
            };
        }
    }
}