using System.Collections.Generic;

namespace Transformalize.Libs.GoogleMaps.LocationServices {

    public class Directions {

        public enum Status {
            Ok,
            Failed
        }

        public Directions() {
            Steps = new List<Step>();
        }

        public List<Step> Steps { get; set; }
        public string Duration { get; set; }
        public string Distance { get; set; }

        public Status StatusCode { get; set; }
    }
}
