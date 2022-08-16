using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models.Tables
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public int MakeId { get; set; } // refs Make.MakeId
        public int ModelId { get; set; } // refs Model.ModelId
        public int BodyStyleId { get; set; } // refs BodyStyle.BodyStyleId
        public int Year { get; set; }
        public int TransmissionId { get; set; } // refs Transmission.TransmissionId
        public int ExteriorId { get; set; } // refs Exterior.ExteriorId
        public int InteriorId { get; set; } // refs Interior.InteriorId
        public int Mileage { get; set; }
        public string VIN { get; set; }
        public decimal MSRP { get; set; }
        public decimal SalePrice { get; set; }
        public string Description { get; set; }
        public string ImageFileName { get; set; } // image should be saved as 'inventory-x-y.ext'; x = VehicleId, y = VIN
        public int ListingStatusId { get; set; } // refs ListingStatus.ListingStatusId
    }
}
