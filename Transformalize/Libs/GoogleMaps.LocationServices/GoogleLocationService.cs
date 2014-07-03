using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Transformalize.Libs.GoogleMaps.LocationServices {
    public class GoogleLocationService : ILocationService {

        const string API_REGION_FROM_LATLONG = "maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
        const string API_LATLONG_FROM_ADDRESS = "maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false";
        const string API_DIRECTIONS = "maps.googleapis.com/maps/api/directions/xml?origin={0}&destination={1}&sensor=false";

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleLocationService"/> class.
        /// </summary>
        /// <param name="useHttps">Indicates whether to call the Google API over HTTPS or not.</param>
        public GoogleLocationService(bool useHttps) {
            UseHttps = useHttps;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleLocationService"/> class. Default calling the API over regular
        /// HTTP (not HTTPS).
        /// </summary>
        public GoogleLocationService()
            : this(false) { }

        /// <summary>
        /// Gets a value indicating whether to use the Google API over HTTPS.
        /// </summary>
        /// <value>
        ///   <c>true</c> if using the API over HTTPS; otherwise, <c>false</c>.
        /// </value>
        public bool UseHttps { get; private set; }

        private string UrlProtocolPrefix {
            get {
                return UseHttps ? "https://" : "http://";
            }
        }

        protected string ApiUrlRegionFromLatLong {
            get {
                return UrlProtocolPrefix + API_REGION_FROM_LATLONG;
            }
        }

        protected string ApiUrlLatLongFromAddress {
            get {
                return UrlProtocolPrefix + API_LATLONG_FROM_ADDRESS;
            }
        }

        protected string ApiUrlDirections {
            get {
                return UrlProtocolPrefix + API_DIRECTIONS;
            }
        }

        /// <summary>
        /// Translates a Latitude / Longitude into a Region (state) using Google Maps api
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public Region GetRegionFromLatLong(double latitude, double longitude) {
            var doc = XDocument.Load(string.Format(ApiUrlRegionFromLatLong, latitude, longitude));
            var els = doc.Descendants("result").First().Descendants("address_component").FirstOrDefault(s => s.Descendants("type").First().Value == "administrative_area_level_1");
            return null != els ? new Region() { Name = els.Descendants("long_name").First().Value, ShortCode = els.Descendants("short_name").First().Value } : null;
        }

        /// <summary>
        /// Gets the latitude and longitude that belongs to an address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public MapPoint GetLatLongFromAddress(string address) {
            var doc = XDocument.Load(string.Format(ApiUrlLatLongFromAddress, Uri.EscapeDataString(address)));
            var els = doc.Descendants("result").Descendants("geometry").Descendants("location").FirstOrDefault();
            if (null == els)
                return null;
            var xElement = els.Nodes().First() as XElement;
            if (xElement == null)
                return null;
            var latitude = ParseUs(xElement.Value);
            var element = els.Nodes().ElementAt(1) as XElement;
            if (element == null)
                return null;
            var longitude = ParseUs(element.Value);
            return new MapPoint() { Latitude = latitude, Longitude = longitude };
        }

        /// <summary>
        /// Gets the latitude and longitude that belongs to an address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public MapPoint GetLatLongFromAddress(AddressData address) {
            return GetLatLongFromAddress(address.ToString());
        }


        /// <summary>
        /// Gets the directions.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Directions GetDirections(double latitude, double longitude) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the directions.
        /// </summary>
        /// <param name="originAddress">From address.</param>
        /// <param name="destinationAddress">To address.</param>
        /// <returns>The directions</returns>
        public Directions GetDirections(AddressData originAddress, AddressData destinationAddress) {
            var direction = new Directions();

            var xdoc = XDocument.Load(String.Format(ApiUrlDirections,
                Uri.EscapeDataString(originAddress.ToString()),
                Uri.EscapeDataString(destinationAddress.ToString())));

            var status = (from s in xdoc.Descendants("DirectionsResponse").Descendants("status")
                          select s).FirstOrDefault();

            if (status != null && status.Value == "OK") {
                direction.StatusCode = Directions.Status.Ok;
                var distance = (from d in xdoc.Descendants("DirectionsResponse").Descendants("route").Descendants("leg")
                               .Descendants("distance").Descendants("text")
                                select d).LastOrDefault();

                if (distance != null) {
                    direction.Distance = distance.Value;
                }

                var duration = (from d in xdoc.Descendants("DirectionsResponse").Descendants("route").Descendants("leg")
                               .Descendants("duration").Descendants("text")
                                select d).LastOrDefault();

                if (duration != null) {
                    direction.Duration = duration.Value;
                }

                var steps = from s in xdoc.Descendants("DirectionsResponse").Descendants("route").Descendants("leg").Descendants("step")
                            select s;

                foreach (var step in steps) {
                    var xElement = step.Element("html_instructions");
                    if (xElement == null)
                        continue;
                    var element = step.Descendants("distance").First().Element("text");
                    if (element == null)
                        continue;
                    var directionStep = new Step {
                        Instruction = xElement.Value,
                        Distance = element.Value
                    };
                    direction.Steps.Add(directionStep);
                }
                return direction;
            }
            if (status == null || status.Value == "OK")
                throw new Exception("Unable to get Directions from Google");
            direction.StatusCode = Directions.Status.Failed;
            return direction;
        }

        static double ParseUs(string value) {
            return Double.Parse(value, new CultureInfo("en-US"));
        }
    }
}
