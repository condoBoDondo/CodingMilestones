using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models.Tables
{
    public class Sale
    {
        public int SaleId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateId { get; set; } // refs State.StateId
        public string Zip { get; set; }
        public int VehicleId { get; set; } // refs Vehicle.VehicleId
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PurchaseTypeId { get; set; } // refs PurchaseType.PurchaseTypeId
        public string Salesperson { get; set; } // refs AspNetUsers.Id
    }
}
