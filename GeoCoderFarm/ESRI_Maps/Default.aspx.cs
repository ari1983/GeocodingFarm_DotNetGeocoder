using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ESRI_Maps
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string sqlText = "SELECT AssetID, Title, Address, Lat, Lng FROM tbl_Assets";

                using (var cmd = new SqlCommand(sqlText, conn))
                {
                    conn.Open();
                    var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    rptAssetList.DataSource = reader;
                    rptAssetList.DataBind();

                    reader.Close();
                }
            }
        }
    }
}