using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using AM.Geocoding;
using AM.Geocoding.GeocodeFarm;
using AM.DAL;

namespace ESRI_Maps
{
    /// <summary>
    /// Accepts requests and returns data in XML format
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ArcGisWebService : System.Web.Services.WebService
    {
        /// <summary>
        /// Default Ctor
        /// </summary>
        public ArcGisWebService()
        {
            this.geoService = new GeocodeFarmService("119e07c74a9d2fa7ed641be566c3e07a197d871e");
            this.dataContext = new DataContext();
        }

        /// <summary>
        /// Injected Ctor
        /// </summary>
        /// <param name="geoService">Instance of IGeocoderService</param>
        /// <param name="dataContext">Facade to DB (interface must be extracted !!!)</param>
        public ArcGisWebService(IGeocoderService geoService, DataContext dataContext)
        {
            this.geoService = geoService;
            this.dataContext = dataContext;
        }

        /// <summary>
        /// Performs batch geocoding and returns response in XML-text format
        /// </summary>
        [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Xml, UseHttpGet = true)]
        public string GetGeocodeFarmResponse()
        {
            // Reading all Addresses from DB to resolve into {x, y}'s
            IEnumerable<string> addresses = dataContext.GetAllAddresses();

            // BATCH GeoCoding
            IEnumerable<GeocodingResponse> responseList = geoService.PerformBulkForwardGeocoding(addresses);

            if (responseList.Count() > 0)
            {
                //
                // List of Assets is joined with the list of {x, y} from response
                // REMARK: as soon as we get back geocoded results, we need to correlate them against the list of Assets
                // as we don't know which {x, y} relates to certain Asset
                var joinedResponse =
                                    from response in responseList
                                    join asset in dataContext.GetAllAssets()
                                    on response.AddressProvided equals asset.Address.ToUpper()
                                    select new
                                            {
                                                AssetID = asset.AssetID,
                                                Title = asset.Title,
                                                Description = asset.Description,
                                                Address = asset.Address,
                                                Image = asset.Image,
                                                Lat = response.Lat,
                                                Lng = response.Lng
                                            };

                // concatenating result string
                var str = new StringBuilder();

                foreach (var r in joinedResponse)
                    str.Append(r.Lat + "|" + r.Lng + "|" + r.Title + "|" + r.Description + "|" + r.Address + "|" + r.Image + ";"); // Format: x|y|Title|Description|Address;

                // removing trailing ";" character from the result string
                return str.ToString().Substring(0, str.Length - 1);
            }

            return string.Empty;
        }

        // private fields
        private IGeocoderService geoService;
        private DataContext dataContext;
    }
}
