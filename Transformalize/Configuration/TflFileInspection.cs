using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

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

        [Cfg(required = false)]
        public List<TflType> Types { get; set; }
        [Cfg(required = true)]
        public List<TflDelimiter> Delimiters { get; set; }
    }
}