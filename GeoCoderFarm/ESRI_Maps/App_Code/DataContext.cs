using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AM.DAL
{
    /// <summary>
    /// Entry point to the Database entitites
    /// </summary>
    public class DataContext
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public DataContext()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        /// <summary>
        /// Retrieves all addresses from 'Assets' table
        /// </summary>
        /// <returns>List of addresses</returns>
        public IList<string> GetAllAddresses()
        {
            IList<string> addresses = new List<string>();

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("SELECT Address FROM tbl_Assets", conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                addresses.Add(reader["Address"].ToString());
                    }
                }
            }

            return addresses;
        }

        /// <summary>
        /// Retrieves all 'Assets' entities from corresponding table
        /// </summary>
        /// <returns>List of Assets entities</returns>
        public IList<AssetEntity> GetAllAssets()
        {
            IList<AssetEntity> assetList = new List<AssetEntity>();

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("SELECT AssetID, Title, Address, Description, Image, Lat, Lng FROM tbl_Assets", conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var asset = new AssetEntity()
                                {
                                    AssetID = Int32.Parse(reader["AssetID"].ToString()),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    Image = reader["Image"].ToString(),
                                    Lat = reader["Lat"].ToString(),
                                    Lng = reader["Lng"].ToString()
                                };

                                assetList.Add(asset);
                            }
                        }
                    }
                }
            }

            return assetList;
        }

        /// <summary>
        /// Retrieves a single Asset entity by its Address value
        /// </summary>
        /// <param name="address">Address of the Asset</param>
        /// <returns>Asset entity</returns>
        public AssetEntity GetAssetByAddress(string address)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("SELECT TOP(1) AssetID, Title, Address, Lat, Lng FROM tbl_Assets WHERE Address = @addressParam", conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@addressParam", address);

                    using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return new AssetEntity()
                            {
                                AssetID = Int32.Parse(reader["AssetID"].ToString()),
                                Title = reader["Title"].ToString(),
                                Address = reader["Address"].ToString(),
                                Lat = reader["Lat"].ToString(),
                                Lng = reader["Lng"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Updates Asset entity by its ID
        /// </summary>
        /// <param name="id">ID of the Asset</param>
        public void UpdateAssetById(int id)
        {
            throw new NotImplementedException("Update method isn't implemented...");
        }
        // private vars
        private string connectionString;
    }
}