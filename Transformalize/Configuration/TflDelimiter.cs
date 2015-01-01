namespace Transformalize.Configuration {

    public class TflDelimiter : CfgNode {
        public TflDelimiter() {
            Property(n:"character", v:default(char), r:true, u:true, d:true);
            Property(n:"name", v:string.Empty, r:true);
        }

        public char Character { get; set; }
        public string Name { get; set; }
    }
}