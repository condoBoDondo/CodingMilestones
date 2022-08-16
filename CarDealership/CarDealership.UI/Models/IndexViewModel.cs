using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarDealership.UI.Models
{
    public class IndexViewModel
    {
        // contains models used on index page
        public IEnumerable<VehicleFeatured> Vehicles { get; set; }
        public IEnumerable<Special> Specials { get; set; }
    }
}