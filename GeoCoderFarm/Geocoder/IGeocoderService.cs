using System;
using System.Collections.Generic;

namespace AM.Geocoding
{
    /// <summary>
    /// Defines methods for forward/reverse single/batch geocoding
    /// </summary>
    public interface IGeocoderService
    {
        /// <summary>
        /// Performs single address forward geocoding
        /// </summary>
        /// <param name="addressToResolve">Address line</param>
        /// <returns>ForwardGeocodingResponse object with response data</returns>
        GeocodingResponse PerformSingleForwardGeocoding(string addressToResolve);

        /// <summary>
        /// Performs multiple addresses forward geocoding
        /// </summary>
        /// <param name="addressesToResolve">List of addresses</param>
        /// <returns>List of ForwardGeocodingResponse objects containing response data</returns>
        IEnumerable<GeocodingResponse> PerformBulkForwardGeocoding(IEnumerable<string> addressesToResolve);

        /// <summary>
        /// Performs single geo-point reverse geocoding
        /// </summary>
        /// <param name="geoPointToResolve">Pair of latitude and longitude</param>
        /// <returns>ReverseGeocodingResponse object with response data</returns>
        GeocodingResponse PerformSingleReverseGeocoding(GeoPoint geoPointToResolve);

        /// <summary>
        /// Performs multiple geo-points reverse geocoding
        /// </summary>
        /// <param name="geoPointsToResolve">List of geo-points</param>
        /// <returns>List of ReverseGeocodingResponse objects containing response data</returns>
        IEnumerable<GeocodingResponse> PerformBulkReverseGeocoding(IEnumerable<GeoPoint> geoPointsToResolve);
    }
}