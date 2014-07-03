using System.ComponentModel.DataAnnotations;

namespace Transformalize.Libs.GoogleMaps.LocationServices {
    public class Region {
        public string Name { get; set; }
        [StringLength(2, MinimumLength = 2)]
        public string ShortCode { get; set; }
    }
}
