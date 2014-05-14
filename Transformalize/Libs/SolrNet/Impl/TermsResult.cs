using System.Collections.Generic;

namespace Transformalize.Libs.SolrNet.Impl {
    /// <summary>
    /// Terms Results
    /// </summary>
    public class TermsResult {
        /// <summary>
        /// terms field
        /// </summary>
        public string Field { get; set;}

        /// <summary>
        /// Spelling suggestions
        /// </summary>
        public ICollection<KeyValuePair<string,int>> Terms { get; set; }
    }
}