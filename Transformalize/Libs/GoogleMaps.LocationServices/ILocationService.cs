namespace Transformalize.Libs.GoogleMaps.LocationServices
{
    public interface ILocationService
    {
        /// <summary>
        /// Translates a Latitude / Longitude into a Region (state) using Google Maps api
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        Region GetRegionFromLatLong(double latitude, double longitude);


        /// <summary>
        /// Gets the latitude and longitude that belongs to an address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        MapPoint GetLatLongFromAddress(string address);


        /// <summary>
        /// Gets the directions.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>The direction.</returns>
        Directions GetDirections(double latitude, double longitude);

        /// <summary>
        /// Gets the directions.
        /// </summary>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddress">To address.</param>
        /// <returns>The directions</returns>
        Directions GetDirections(AddressData fromAddress, AddressData toAddress);

    }
}
