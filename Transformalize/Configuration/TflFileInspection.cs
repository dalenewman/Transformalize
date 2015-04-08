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
        [Cfg(value = 100)]
        public int LineLimit { get; set; }

        [Cfg(required = false)]
        public List<TflType> Types { get; set; }
        [Cfg(required = true)]
        public List<TflDelimiter> Delimiters { get; set; }

        public FileInspectionRequest GetInspectionRequest(string fileName) {

            return new FileInspectionRequest(fileName) {
                DataTypes = Types.Select(t => t.Type).ToList(),
                DefaultLength = MinLength == 0 ? "1024" : MinLength.ToString(CultureInfo.InvariantCulture),
                DefaultType = "string",
                MinLength = MinLength,
                LineLimit = LineLimit,
                MaxLength = MaxLength,
                Delimiters = Delimiters.ToDictionary(d => d.Character, d => d),
                Sample = Sample,
                IgnoreEmpty = IgnoreEmpty
            };
        }
    }
}