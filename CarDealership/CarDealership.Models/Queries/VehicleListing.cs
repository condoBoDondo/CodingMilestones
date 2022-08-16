﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Models.Queries
{
    // page-width listing used on all vehicle search pages; no description
    public class VehicleListing
    {
        public int VehicleId { get; set; }
        public string MakeName { get; set; }
        public string ModelName { get; set; }
        public string BodyStyleName { get; set; }
        public int Year { get; set; }
        public string TransmissionName { get; set; }
        public string ExteriorName { get; set; }
        public string InteriorName { get; set; }
        public int Mileage { get; set; }
        public string VIN { get; set; }
        public decimal MSRP { get; set; }
        public decimal SalePrice { get; set; }
        public string ImageFileName { get; set; }
        public int ListingStatusId { get; set; }
    }
}
