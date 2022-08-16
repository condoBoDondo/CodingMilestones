using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models.Queries
{
    // small listing used for buttons on main page
    public class VehicleFeatured
    {
        public int VehicleId { get; set; }
        public string MakeName { get; set; }
        public string ModelName { get; set; }
        public int Year { get; set; }
        public decimal SalePrice { get; set; }
        public string ImageFileName { get; set; }
    }
}
