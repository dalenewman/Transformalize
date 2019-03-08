using System.Collections.Generic;
using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Models {
    public class ParameterModel {
        public IEnumerable<Map> Maps { get; set; }
        public Parameter Parameter { get; set; }
    }
}