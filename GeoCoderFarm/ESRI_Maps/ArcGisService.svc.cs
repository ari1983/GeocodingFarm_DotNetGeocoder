using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using AM.Geocoding;
using AM.Geocoding.GeocodeFarm;
using AM.DAL;

namespace ESRI_Maps
{
    /// <summary>
    /// WCF Service for interacting with the ArcGIS maps
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ArcGisService : IGeocodeFarmService
    {
        /// <summary>
        /// Default ctor with hard-coded objects created
        /// </summary>
        public ArcGisService()
        {
            this._geoService = new GeocodeFarmService("119e07c74a9d2fa7ed641be566c3e07a197d871e");
            this._db = new DataContext();
        }

        /// <summary>
        /// Injected ctor for IoC containers
        /// </summary>
        /// <param name="geoService">IGeocoderService instance</param>
        /// <param name="db">Database access entry point</param>
        public ArcGisService(IGeocoderService geoService, DataContext db)
        {
            this._geoService = geoService;
            this._db = db;
        }

        /// <summary>
        /// Retrieves all Assets from DB along with {x, y} coordinates from geocoding web service
        /// </summary>
        /// <returns>List of Assets serialized into XML format</returns>
        public string GetGeocodeFarmResponse()
        {
            // Fetching ALL Addresses from DB
            IEnumerable<string> addresses = _db.GetAllAddresses();

            // Bulk GeoCoding
            IEnumerable<GeocodingResponse> responseList = _geoService.PerformBulkForwardGeocoding(addresses);

            if (responseList.Count() > 0)
            {
                var joinedResponse =
                                    from response in responseList
                                    join asset in _db.GetAllAssets()
                                    on response.AddressProvided equals asset.Address.ToUpper()
                                    select new
                                    {
                                        AssetID = asset.AssetID,
                                        Title = asset.Title,
                                        Description = asset.Description,
                                        Address = asset.Address,
                                        Image = asset.Image,
                                        Lat = response.Lat,
                                        Lng = response.Lng,
                                        RemainingQueries = response.RemainingQueries, // geocoding request remaining
                                        UsedToday = response.UsedToday // geocoding requests used today
                                    };

                var result = new StringBuilder(1000);

                foreach (var r in joinedResponse)
                    result.Append(r.Lat + "|" + r.Lng + "|" + r.Title + "|" + r.Description + "|" + r.Address + "|" + r.Image + "|" + r.UsedToday + "|" + r.RemainingQueries + ";"); // Format: x|y|Title|Description|Address;

                // removing trailing ";" character from the result string
                return result.ToString().Substring(0, result.Length - 1);
            }

            return string.Empty;
        }

        #region Private Vars
        private IGeocoderService _geoService;
        private DataContext _db;
        #endregion
    }
}
