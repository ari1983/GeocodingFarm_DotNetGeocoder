using System;

namespace AM.Geocoding
{
    /// <summary>
    /// Pair of Latitude and Longitude
    /// </summary>
    public struct GeoPoint
    {
        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }
    }
}