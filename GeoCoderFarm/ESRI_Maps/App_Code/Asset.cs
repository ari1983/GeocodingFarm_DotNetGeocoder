using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AM.DAL
{
    /// <summary>
    /// Wrapper class for Asset business entity
    /// </summary>
    public class AssetEntity
    {
        public int AssetID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}