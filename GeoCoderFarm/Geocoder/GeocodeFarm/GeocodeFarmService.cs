using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace AM.Geocoding.GeocodeFarm
{
    /// <summary>
    /// Encapsulates functionality for forward/reverse single/batch geocoding
    /// </summary>
    public class GeocodeFarmService : IGeocoderService
    {
        #region Ctor
        /// <summary>
        /// Parametrized ctor
        /// </summary>
        /// <param name="apiKey">API key</param>
        public GeocodeFarmService(string apiKey)
        {
            this.apiKey = apiKey;
        }
        #endregion

        #region Forward Geocoding Methods

        /// <summary>
        /// Performs single address forward geocoding
        /// </summary>
        /// <param name="addressToResolve">Address line</param>
        /// <returns>GeocodingResponse object with response data</returns>
        public GeocodingResponse PerformSingleForwardGeocoding(string addressToResolve)
        {
            // sending request to GeocodeFarm web service and getting XML response
            XDocument xDoc = this.GetForwardGeocodingXmlResponse(addressToResolve);

            // parsing XML nodes
            IEnumerable<XElement> xmlNode = xDoc.Descendants("STATUS");
            string access           = xmlNode.Descendants("access").First().Value;
            string status           = xmlNode.Descendants("status").First().Value;

            xmlNode                 = xDoc.Descendants("ACCOUNT");
            string name             = xmlNode.Descendants("name").First().Value;
            string email            = xmlNode.Descendants("email").First().Value;
            string apiKey           = xmlNode.Descendants("api_key").First().Value;
            string monthlyDue       = xmlNode.Descendants("monthly_due").First().Value;
            string nextDue          = xmlNode.Descendants("next_due").First().Value;
            string usageLimit       = xmlNode.Descendants("usage_limit").First().Value;
            string used_today       = xmlNode.Descendants("used_today").First().Value;
            string remainingQueries = xmlNode.Descendants("remaining_queries").First().Value;

            xmlNode                 = xDoc.Descendants("ADDRESS");
            string address_provided = xmlNode.Descendants("address_provided").First().Value;
            string address_returned = xmlNode.Descendants("address_returned").First().Value;
            string accuracy         = xmlNode.Descendants("accuracy").First().Value;

            xmlNode                 = xDoc.Descendants("COORDINATES");
            string lat              = xmlNode.Descendants("latitude").First().Value;
            string lng              = xmlNode.Descendants("longitude").First().Value;
 
            return new GeocodingResponse
            {
                Access = access,
                Status = status,
                Name = name,
                Email = email,
                ApiKey = apiKey,
                MonthlyDue = monthlyDue,
                NextDue = nextDue,
                UsageLimit = usageLimit,
                UsedToday = used_today,
                RemainingQueries = remainingQueries,
                AddressProvided = address_provided,
                AddressReturned = address_returned,
                Accuracy = accuracy,
                Lat = lat,
                Lng = lng
            };
        }

        /// <summary>
        /// Performs multiple addresses forward geocoding
        /// </summary>
        /// <param name="addressesToResolve">List of addresses</param>
        /// <returns>List of GeocodingResponse objects with response data</returns>
        public IEnumerable<GeocodingResponse> PerformBulkForwardGeocoding(IEnumerable<string> addressesToResolve)
        {
            var tasks = new List<Task>();
            var responseList = new List<GeocodingResponse>();

            // making list of asynchronous operations
            foreach (var address in addressesToResolve)
                tasks.Add(Task.Factory.StartNew(a => responseList.Add(PerformSingleForwardGeocoding(address)), null));

            // waiting until ALL asynchronous operations are executed
            Task.WaitAll(tasks.ToArray());

            return responseList;
        }

        /// <summary>
        /// Performs forward geocoding request to GeocodeFarm REST service
        /// </summary>
        /// <param name="addressToResolve">Address line</param>
        /// <returns>XML document with response data</returns>
        public XDocument GetForwardGeocodingXmlResponse(string addressToResolve)
        {
            var url = string.Format("http://www.geocodefarm.com/api/forward/xml/{0}/{1}", this.apiKey, addressToResolve);

            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format("Server Error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                    var stream = response.GetResponseStream();
                    XDocument xmlDoc = XDocument.Load(stream);

                    return xmlDoc;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Performs forward geocoding request to GeocodeFarm REST service
        /// </summary>
        /// <param name="addressToResolve">Address line</param>
        /// <returns>JSON string with response data</returns>
        private string GetForwardGeocodingJsonResponse(string addressToResolve)
        {
            var url = string.Format("http://www.geocodefarm.com/api/forward/json/{0}/{1}", this.apiKey, addressToResolve);

            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.ContentType = "application/json; charset=utf-8";
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = "application/json";

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format("Server Error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                    // TODO: replace with REAL response data
                    var stream = response.GetResponseStream();

                    var encoding = Encoding.ASCII;

                    using (var reader = new StreamReader(stream, encoding))
                    {
                        var result = reader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Reverse Geocoding Methods

        /// <summary>
        /// Performs single geo-point reverse geocoding
        /// </summary>
        /// <param name="geoPoint">Pair of latitude and longitude</param>
        /// <returns>ReverseGeocodingResponse object with response data</returns>
        public GeocodingResponse PerformSingleReverseGeocoding(GeoPoint geoPoint)
        {
            // sending request to GeocodeFarm web service and getting XML response
            XDocument xDoc = this.GetReverseGeocodingXMLResponse(geoPoint.Latitude, geoPoint.Longitude);

            // parsing XML nodes
            IEnumerable<XElement> xmlNode = xDoc.Descendants("STATUS");
            string access                 = xmlNode.Descendants("access").First().Value;
            string status                 = xmlNode.Descendants("status").First().Value;

            xmlNode                       = xDoc.Descendants("ACCOUNT");
            string name                   = xmlNode.Descendants("name").First().Value;
            string email                  = xmlNode.Descendants("email").First().Value;
            string apiKey                 = xmlNode.Descendants("api_key").First().Value;
            string monthlyDue             = xmlNode.Descendants("monthly_due").First().Value;
            string nextDue                = xmlNode.Descendants("next_due").First().Value;
            string usageLimit             = xmlNode.Descendants("usage_limit").First().Value;
            string used_today             = xmlNode.Descendants("used_today").First().Value;
            string remainingQueries       = xmlNode.Descendants("remaining_queries").First().Value;

            xmlNode                       = xDoc.Descendants("ADDRESS");
            string address_returned       = xmlNode.Descendants("address").First().Value;
            string accuracy               = xmlNode.Descendants("accuracy").First().Value;

            xmlNode                       = xDoc.Descendants("COORDINATES");
            string lat                    = xmlNode.Descendants("latitude").First().Value;
            string lng                    = xmlNode.Descendants("longitude").First().Value;


            return new GeocodingResponse
            {
                Access = access,
                Status = status,
                Name = name,
                Email = email,
                ApiKey = apiKey,
                MonthlyDue = monthlyDue,
                NextDue = nextDue,
                UsageLimit = usageLimit,
                UsedToday = used_today,
                RemainingQueries = remainingQueries,
                AddressReturned = address_returned,
                Accuracy = accuracy,
                Lat = lat,
                Lng = lng
            };
        }

        /// <summary>
        /// Performs multiple geo-points reverse geocoding
        /// </summary>
        /// <param name="geoPoints">List of geo-points</param>
        /// <returns>List of ReverseGeocodingResponse objects containing response data</returns>
        public IEnumerable<GeocodingResponse> PerformBulkReverseGeocoding(IEnumerable<GeoPoint> geoPoints)
        {
            var tasks = new List<Task>();
            var responseList = new List<GeocodingResponse>();

            // making list of asyncronous operations
            foreach (var geoPoint in geoPoints)
                tasks.Add(Task.Factory.StartNew(a => responseList.Add(PerformSingleReverseGeocoding(geoPoint)), null));

            // waiting untill ALL asyncronous operations are executed
            Task.WaitAll(tasks.ToArray());

            return responseList;
        }

        /// <summary>
        /// Performs reverse geocoding request to GeocodeFarm REST service
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>XML document with response data</returns>
        public XDocument GetReverseGeocodingXMLResponse(double latitude, double longitude)
        {
            var url = string.Format("http://www.geocodefarm.com/api/reverse/xml/{0}/{1}/{2}", this.apiKey, latitude, longitude);

            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format("Server Error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                    var stream = response.GetResponseStream();
                    XDocument xmlDoc = XDocument.Load(stream);

                    return xmlDoc;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Performs reverse geocoding request to GeocodeFarm REST service
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>JSON string with response data</returns>
        private string GetReverseGeocodingJsonResponse(double latitude, double longitude)
        {
            var url = string.Format("http://www.geocodefarm.com/api/reverse/xml/{0}/{1}/{2}", this.apiKey, latitude, longitude);

            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format("Server Error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                    // TODO: replace with REAL response data
                    var stream = response.GetResponseStream();

                    var encoding = Encoding.ASCII;

                    using (var reader = new StreamReader(stream, encoding))
                    {
                        var result = reader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Private Variables
        /// <summary>
        /// API Key
        /// </summary>
        private string apiKey;
        #endregion
    }
}