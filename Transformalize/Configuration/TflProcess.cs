using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;
using Transformalize.Main.Transform;

namespace Transformalize.Configuration {
    public class TflProcess : CfgNode {

        /// <summary>
        /// A name (of your choosing) to identify the process.
        /// </summary>
        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// `True` by default.
        /// 
        /// Indicates the process is enabled.  The included executable (e.g. `tfl.exe`) 
        /// respects this setting and does not run the process if disabled (or `False`).
        /// </summary>
        [Cfg(value = true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Optional. 
        /// 
        /// A mode reflects the intent of running the process.
        ///  
        /// * `init` wipes everything out
        /// * <strong>`default`</strong> moves data through the pipeline, from input to output.
        /// 
        /// Aside from these, you may use any mode (of your choosing).  Then, you can control
        /// whether or not templates and/or actions run by setting their modes.
        /// </summary>
        [Cfg(value = "")]
        public string Mode { get; set; }

        /// <summary>
        /// Optional.  Default is `false`
        /// 
        /// If true, process entities in parallel.  If false, process them one by one in their configuration order.
        /// 
        /// Parallel *on* allows you to process all the entities at the same time, potentially faster.
        /// Parallel *off* allows you to have one entity depend on a previous entity's data.
        /// </summary>
        [Cfg(value = false)]
        public bool Parallel { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// A choice between `MultiThreaded`, `SingleThreaded`, and <strong>`Default`</strong>.
        /// 
        /// `Default` defers this decision to the entity's PipelineThreading setting.
        /// </summary>
        [Cfg(value = "Default", domain = "SingleThreaded,MultiThreaded,Default")]
        public string PipelineThreading { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// If your output is a relational database that supports views and `StarEnabled` is `True`,
        /// this is the name of a view that projects fields from all the entities in the
        /// star-schema as a single flat projection.
        /// 
        /// If not set, it is the combination of the process name, and "Star." 
        /// </summary>
        [Cfg(value = "")]
        public string Star { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// `True` by default.
        /// 
        /// Star refers to star-schema Transformalize is creating.  You can turn this off 
        /// if your intention is not to create a star-schema.  A `False` setting here may
        /// speed things up.
        /// </summary>
        [Cfg(value = true)]
        public bool StarEnabled { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// Choices are `html` and <strong>`raw`</strong>.
        /// 
        /// This refers to the razor templating engine's content type.  If you're rendering HTML 
        /// markup, use `html`, if not, using `raw` may inprove performance.
        /// </summary>
        [Cfg(value = "raw", domain = "raw,html")]
        public string TemplateContentType { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// Indicates the data's time zone.
        /// 
        /// It is used as the `to-time-zone` setting for `now()` and `timezone()` transformations
        /// if the `to-time-zone` is not set.
        /// 
        /// NOTE: Normally, you should keep the dates in UTC until presented to the user. 
        /// Then, have the client application convert UTC to the user's time zone.
        /// </summary>
        [Cfg(value = "")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// If your output is a relational database that supports views, this is the name of
        /// a view that projects fields from all the entities.  This is different from 
        /// the Star view, as it's joins are exactly as configured in the <relationships/> 
        /// collection.
        /// 
        /// If not set, it is the combination of the process name, and "View." 
        /// </summary>
        [Cfg(value = "")]
        public string View { get; set; }

        [Cfg(value = false)]
        public bool ViewEnabled { get; set; }

        /// <summary>
        /// A collection of [Actions](/action)
        /// </summary>
        [Cfg()]
        public List<TflAction> Actions { get; set; }

        /// <summary>
        /// A collection of [Calculated Fields](/calculated-field)
        /// </summary>
        [Cfg()]
        public List<TflField> CalculatedFields { get; set; }

        /// <summary>
        /// A collection of [Connections](/connection)
        /// </summary>
        [Cfg(required = true)]
        public List<TflConnection> Connections { get; set; }

        /// <summary>
        /// A collection of [Entities](/entity)
        /// </summary>
        [Cfg(required = true)]
        public List<TflEntity> Entities { get; set; }

        /// <summary>
        /// Settings to control [file inspection](/file-inspection).
        /// </summary>
        [Cfg()]
        public List<TflFileInspection> FileInspection { get; set; }

        /// <summary>
        /// A collection of [Logs](/log)
        /// </summary>
        [Cfg(sharedProperty = "rows", sharedValue = (long)10000)]
        public List<TflLog> Log { get; set; }

        /// <summary>
        /// A collection of [Maps](/map)
        /// </summary>
        [Cfg()]
        public List<TflMap> Maps { get; set; }

        /// <summary>
        /// A collection of [Relationships](/relationship)
        /// </summary>
        [Cfg()]
        public List<TflRelationship> Relationships { get; set; }

        /// <summary>
        /// A collection of [Scripts](/script)
        /// </summary>
        [Cfg(sharedProperty = "path", sharedValue = "")]
        public List<TflScript> Scripts { get; set; }

        /// <summary>
        /// A collection of [Search Types](/search-type)
        /// </summary>
        [Cfg()]
        public List<TflSearchType> SearchTypes { get; set; }

        /// <summary>
        /// A collection of [Templates](/template)
        /// </summary>
        [Cfg(sharedProperty = "path", sharedValue = "")]
        public List<TflTemplate> Templates { get; set; }

        protected override void Modify() {

            if (String.IsNullOrEmpty(Star)) {
                Star = Name + "Star";
            }

            if (string.IsNullOrEmpty(View)) {
                View = Name + "View";
            }

            foreach (var calculatedField in CalculatedFields) {
                calculatedField.Input = false;
            }

            try {
                AdaptFieldsCreatedFromTransforms(new[] { "fromxml", "fromregex", "fromjson", "fromsplit" });
            } catch (Exception ex) {
                AddProblem("Trouble adapting fields created from transforms. {0}", ex.Message);
            }

            try {
                var factory = new ShortHandFactory(this);
                factory.ExpandShortHandTransforms();
            } catch (Exception ex) {
                AddProblem("Trouble expanding short hand transforms. {0}", ex.Message);
            }
        }


        private void AdaptFieldsCreatedFromTransforms(IEnumerable<string> transformToFields) {
            foreach (var field in transformToFields) {
                while (new TransformFieldsToParametersAdapter(this).Adapt(field) > 0) {
                    new TransformFieldsMoveAdapter(this).Adapt(field);
                }
            }
        }

        protected override void Validate() {
            ValidateDuplicateEntities();
            ValidateDuplicateFields();
            ValidateLogConnections();
        }

        private void ValidateLogConnections() {
            if (Log.Count <= 0)
                return;

            foreach (var log in Log.Where(log => log.Connection != Common.DefaultValue).Where(log => Connections.All(c => c.Name != log.Connection))) {
                AddProblem(string.Format("Log {0}'s connection {1} doesn't exist.", log.Name, log.Connection));
            }
        }

        private void ValidateDuplicateFields() {
            var fieldDuplicates = Entities
                .SelectMany(e => e.AllFields())
                .Where(f => !f.PrimaryKey)
                .Union(CalculatedFields)
                .GroupBy(f => f.Alias)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToArray();
            foreach (var duplicate in fieldDuplicates) {
                AddProblem(
                    string.Format(
                        "The entity field '{0}' occurs more than once. Remove, alias, or prefix one.",
                        duplicate));
            }
        }

        private void ValidateDuplicateEntities() {
            var entityDuplicates = Entities
                .GroupBy(e => e.Alias)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToArray();
            foreach (var duplicate in entityDuplicates) {
                AddProblem(string.Format("The '{0}' entity occurs more than once. Remove or alias one.", duplicate));
            }
        }
    }
}