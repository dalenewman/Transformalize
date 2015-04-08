using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Configuration {
    public class TflEntity : CfgNode {

        [Cfg(required = false, unique = true)]
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

        /// <summary>
        /// Optional.  Defaults to `SingleThreaded`.
        /// 
        /// Controls the threading of Rhino ETL's pipeline. 
        /// Valid values are `SingleThreaded`, and `MultiThreaded`.
        /// `MultiThreaded` can be faster depending on the computer, but may obscure error messages in some cases.
        /// 
        /// **Note**: You can set each entity if you want, or control all entities from the Process' pipeline threading attribute.
        /// 
        /// In general, you should develop using `SingleThreaded`, and once everything is stable, switch over to `MultiThreaded`.
        /// </summary>
        [Cfg(value = "SingleThreaded", domain = "SingleThreaded,MultiThreaded", ignoreCase = true)]
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
        [Cfg(value = true)]
        public bool Unicode { get; set; }
        [Cfg(value = true)]
        public bool VariableLength { get; set; }
        [Cfg(value = "")]
        public string Version { get; set; }

        [Cfg(required = false)]
        public List<TflFilter> Filter { get; set; }
        [Cfg(required = false)]
        public List<TflField> Fields { get; set; }
        [Cfg(required = false)]
        public List<TflField> CalculatedFields { get; set; }
        [Cfg(required = false)]
        public List<TflIo> Input { get; set; }
        [Cfg(required = false)]
        public List<TflIo> Output { get; set; }

        public IOperation InputOperation { get; set; }

        public IEnumerable<TflField> AllFields() {
            foreach (var f in Fields) {
                yield return f;
                foreach (var transform in f.Transforms) {
                    foreach (var tf in transform.Fields) {
                        yield return tf;
                    }
                }
            }
            foreach (var calculatedField in CalculatedFields) {
                yield return calculatedField;
            }
        }

        protected override void Modify() {
            if (string.IsNullOrEmpty(Alias)) {
                Alias = Name;
            }
            foreach (var calculatedField in CalculatedFields) {
                calculatedField.Input = false;
            }
            if (!string.IsNullOrEmpty(Prefix)) {
                foreach (var field in Fields.Where(f => f.Alias == f.Name)) {
                    field.Alias = Prefix + field.Name;
                }
            }
        }

        protected override void Validate() {
            var fields = AllFields().ToArray();
            var names = new HashSet<string>(fields.Select(f => f.Name).Distinct());
            var aliases = new HashSet<string>(fields.Select(f => f.Alias));

            ValidateVersion(names, aliases);
        }

        private void ValidateVersion(HashSet<string> names, HashSet<string> aliases) {
            if (Version == string.Empty)
                return;

            if (names.Contains(Version))
                return;

            if (aliases.Contains(Version))
                return;

            AddProblem("Cant't find version field '{0}' in entity '{1}'", Version, Name);
        }

        private void ValidateFilter(HashSet<string> names, HashSet<string> aliases) {
            if (Filter.Count == 0)
                return;

            foreach (var f in Filter) {
                if (f.Expression != string.Empty)
                    return;

                if (names.Contains(f.Left))
                    continue;

                if (aliases.Contains(f.Left))
                    continue;

                AddProblem("A filter's left attribute must reference a defined field. '{0}' is not defined.", f.Left);
            }
        }
    }
}