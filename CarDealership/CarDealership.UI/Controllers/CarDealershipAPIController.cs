using CarDealership.Data.Factories;
using CarDealership.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarDealership.UI.Controllers
{
    public class CarDealershipAPIController : ApiController
    {
        // these are necessary for ajax calls done by views

        // get vehicles for search pages
        [Route("api/vehicle/search")]
        [AcceptVerbs("GET")]
        public IHttpActionResult SearchVehicles(string searchInput, decimal? minPrice, decimal? maxPrice, int? minYear, int? maxYear, string mode)
        {
            var repo = VehicleRepositoryFactory.GetRepository();

            try
            {
                var parameters = new VehicleSearchParameters()
                {
                    Input = searchInput,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    MinYear = minYear,
                    MaxYear = maxYear
                };

                var result = repo.Search(parameters, mode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get list of models by matching makeId
        [Route("api/vehicle/modelByMake")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetModelsByMakeId(int makeId)
        {
            var repo = ModelRepositoryFactory.GetRepository();

            try
            {
                var result = repo.GetByMakeId(makeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get list of sales by filter
        [Route("api/report/sale")]
        [AcceptVerbs("GET")]
        public IHttpActionResult SearchSalesReports(string salesperson, DateTime? minDate, DateTime? maxDate)
        {
            var repo = SaleRepositoryFactory.GetRepository();

            try
            {
                var parameters = new SaleReportSearchParameters()
                {
                    Salesperson = salesperson,
                    MinDate = minDate,
                    MaxDate = maxDate
                };

                var result = repo.GetReports(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}