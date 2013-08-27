using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using AM.Geocoding;
using AM.Geocoding.GeocodeFarm;

namespace ESRI_Maps
{
    public partial class ViewAsset : System.Web.UI.Page
    {
        // Geocoder object
        private GeocodeFarmService geoService;

        // Ctor
        public ViewAsset()
        {
            geoService = new GeocodeFarmService("119e07c74a9d2fa7ed641be566c3e07a197d871e");
        }

        // Page Initialization code
        protected void Page_Init(object sender, EventArgs e)
        {
            int assetID = 0;

            try
            {
                assetID = Int32.Parse(Request.QueryString["AssetID"]);
            }
            catch (Exception ex)
            {
                Response.Redirect("NotFound.aspx");
            }

            // DB access
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string sqlQuery = "SELECT Title, Address, Lat, Lng FROM tbl_Assets WHERE AssetID = @AssetID";

                using (var cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AssetID", assetID);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            string address = reader["Address"].ToString();
                            GeocodingResponse response = geoService.PerformSingleForwardGeocoding(address); // geocoding processing
 
                            // populating hidden fields with Latitude & Longitude values
                            hdnLat.Value = response.Lat.ToString();
                            hdnLng.Value = response.Lng.ToString();
                        }
                    }
                }
            }
        }
    }
}