using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models.Queries
{
    // used for admin-accessed reports of vehicle sales (aggregates)
    public class SaleReport
    {
        public string UserId { get; set; } // user who made the sales
        public string FirstLastName { get; set; } // first/last name together
        public string Email { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalVehicles { get; set; }
    }
}
