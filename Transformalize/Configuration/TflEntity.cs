using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Configuration {
    public class TflEntity : CfgNode {

        [Cfg(value = "", required = false, unique = true)]
        public string Alias { get; set; }
        [Cfg(value = "input")]
        public string Connection { get; set; }
        [Cfg(value = false)]
        public bool Delete { get; set; }

        /// <summary>
        /// Optional : `True` by default.
        /// 
        /// Currently this is a confusing option.  It's ambiguous:
        /// 
        /// * Does it mean "detect changes" between the input and output?
        ///   * If true, TFL will attempt to insert or update data, if a version field is available.  
        ///   * If false, TFL will only insert data, it will not compare input with output to `insert` or `update`.
        /// 
        /// * Does it affect what is loaded from the input
        ///   * If true, and input is capable of querying, and output has previous version value, TFL will pull delta from the input
        ///   * If false, TFL will not attempt to pull delta from input.
        /// 
        /// ###Ideas
        /// 
        /// * Add CanQuery to connection (true or false).  It's all queryable, just a matter of whether or not you have to load everything into memory.
        /// * This was mostly added to deal with importing single files.  If file connection was implemented to detect changes, might not need this. 
        /// * There are two concepts, querying just the delta from the input, and comparing the input and output, which requires a version and loading the corresponding output keys and version
        /// 
        /// </summary>
        [Cfg(value = true)]
        public bool DetectChanges { get; set; }

        [Cfg(value = false)]
        public bool Group { get; set; }
        [Cfg(value = "", required = true)]
        public string Name { get; set; }
        [Cfg(value = false)]
        public bool NoLock { get; set; }

        [Cfg(value = "Default", domain = "SingleThreaded,MultiThreaded,Default")]
        public string PipelineThreading { get; set; }
        [Cfg(value = "")]
        public string Prefix { get; set; }
        [Cfg(value = true)]
        public bool PrependProcessNameToOutputName { get; set; }
        [Cfg(value = "")]
        public string Query { get; set; }
        [Cfg(value = "")]
        public string QueryKeys { get; set; }
        [Cfg(value = 100)]
        public int Sample { get; set; }
        [Cfg(value = "")]
        public string Schema { get; set; }
        [Cfg(value = "")]
        public string Script { get; set; }
        [Cfg(value = "")]
        public string ScriptKeys { get; set; }
        [Cfg(value = false)]
        public bool TrimAll { get; set; }
        [Cfg(value = "")]
        public string Unicode { get; set; }
        [Cfg(value = "")]
        public string VariableLength { get; set; }
        [Cfg(value = "")]
        public string Version { get; set; }

        [Cfg(required = false)]
        public List<TflFilter> Filters { get; set; }
        [Cfg(required = false)]
        public List<TflField> Fields { get; set; }
        [Cfg(required = false)]
        public List<TflCalculatedField> CalculatedFields { get; set; }
        [Cfg(required = false)]
        public List<TflIo> Input { get; set; }
        [Cfg(required = false)]
        public List<TflIo> Output { get; set; }

        public IOperation InputOperation { get; set; }


    }
}