using CarDealership.Data.Factories;
using CarDealership.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarDealership.UI.Controllers
{
    public class InventoryController : Controller
    {
        public ActionResult New(VehicleSearchParameters parameters)
        {
            var repo = VehicleRepositoryFactory.GetRepository();
            var search = repo.Search(parameters, "new");

            return View(search);
        }

        public ActionResult Used(VehicleSearchParameters parameters)
        {
            var repo = VehicleRepositoryFactory.GetRepository();
            var search = repo.Search(parameters, "used");

            return View(search);
        }

        public ActionResult Details(int id)
        {
            var repo = VehicleRepositoryFactory.GetRepository();
            var model = repo.GetDetails(id);

            return View(model);
        }
    }
}