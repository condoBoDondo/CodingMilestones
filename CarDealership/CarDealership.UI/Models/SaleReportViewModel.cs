using CarDealership.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarDealership.UI.Models
{
    public class SaleReportViewModel
    {
        // get list of all sales reports
        public IEnumerable<SaleReport> SaleReports { get; set; }
    }
}