using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarDealership.UI.Models
{
    public class VehicleReportViewModel
    {
        // need separate lists of both new and used vehicles
        public IEnumerable<VehicleReport> NewVehicleReports { get; set; }
        public IEnumerable<VehicleReport> UsedVehicleReports { get; set; }
    }
}