using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflCalculatedField : TflField {
        [Cfg(value=false)]
        public new bool Input { get; set; }
        [Cfg(value="")]
        public new string Alias { get; set; }
    }
}