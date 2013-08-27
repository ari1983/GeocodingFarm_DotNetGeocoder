using System;

namespace AM.Geocoding
{
    /// <summary>
    ///  Represents response from geocoding service
    /// </summary>
    public class GeocodingResponse
    {
        #region STATUS
        /// <summary>
        /// API key and access permission information
        /// [KEY_VALID, ACCESS_GRANTED]
        /// </summary>
        public string Access { get; set; }

        /// <summary>
        /// Geocoding operation status 
        /// [SUCCESS/FAILURE]
        /// </summary>
        public string Status { get; set; }
        #endregion

        #region ACCOUNT

        /// <summary>
        /// Account holder's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Account holder's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// API key used for the current request
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Amount to pay monthly
        /// </summary>
        public string MonthlyDue { get; set; }

        /// <summary>
        /// Amount to pay next month
        /// </summary>
        public string NextDue { get; set; }

        /// <summary>
        /// Usage limit per day
        /// </summary>
        public string UsageLimit { get; set; }

        /// <summary>
        /// Amount of geocoding requests used today
        /// </summary>
        public string UsedToday { get; set; }

        /// <summary>
        /// Amount of geocoding request remaining for today
        /// </summary>
        public string RemainingQueries { get; set; }
        
        #endregion

        #region ADDRESS
        /// <summary>
        /// Original Address specified to geocode
        /// </summary>
        public string AddressProvided { get; set; }

        /// <summary>
        /// Supplemented Address returned by geocoder
        /// </summary>
        public string AddressReturned { get; set; }

        /// <summary>
        /// Address Resolution precision
        /// [GOOD ACCURACY]
        /// </summary>
        public string Accuracy { get; set; }
        #endregion

        #region COORDINATES
        /// <summary>
        /// Latitude 
        /// [X coordinate]
        /// </summary>
        public string Lat { get; set; }

        /// <summary>
        /// Longitude 
        /// [Y coordinate]
        /// </summary>
        public string Lng { get; set; }
        #endregion
    }
}