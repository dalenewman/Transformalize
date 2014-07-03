using System;

namespace Transformalize.Libs.GoogleMaps.LocationServices {

    public class AddressData {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }

        public override string ToString() {
            return String.Format(
                "{0}{1}{2}{3}{4}",
                Address != null ? Address + ", " : "",
                City != null ? City + ", " : "",
                State != null ? State + ", " : "",
                Zip != null ? Zip + ", " : "",
                Country ?? "").TrimEnd(' ', ',');
        }
    }
}
