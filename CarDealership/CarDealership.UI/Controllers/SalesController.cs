using CarDealership.Data.Factories;
using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using CarDealership.UI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarDealership.UI.Controllers
{
    [Authorize(Roles = "sales,admin")] // sales and admin only
    public class SalesController : Controller
    {
        public ActionResult Index(VehicleSearchParameters parameters)
        {
            var repo = VehicleRepositoryFactory.GetRepository();
            var search = repo.Search(parameters, "sales");

            return View(search);
        }

        public ActionResult Purchase(int id)
        {
            var model = new PurchaseViewModel();

            // get repos required for view model
            var vehicleRepo = VehicleRepositoryFactory.GetRepository();
            var stateRepo = StateRepositoryFactory.GetRepository();
            var purchaseTypeRepo = PurchaseTypeRepositoryFactory.GetRepository();

            model.States = new SelectList(stateRepo.GetAll(), "StateId", "StateId");
            model.PurchaseTypes = new SelectList(purchaseTypeRepo.GetAll(), "PurchaseTypeId", "PurchaseTypeName");
            model.VehicleDetails = vehicleRepo.GetDetails(id);

            model.Sale = new Sale();

            return View(model);
        }

        [HttpPost]
        public ActionResult Purchase(int id, PurchaseViewModel model)
        {
            if (ModelState.IsValid) // was entered data valid?
            {
                // repo to hold new sale
                var saleRepo = SaleRepositoryFactory.GetRepository();

                // get signed-in user id for salesperson
                model.Sale.Salesperson = User.Identity.GetUserId();

                // get correct vehicleId
                // necessary due to some weirdness with saving vehicleId via HiddenFor
                model.Sale.VehicleId = model.VehicleDetails.VehicleId;

                try
                {
                    saleRepo.Purchase(model.Sale); // add sale to repo
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else // data not valid
            {
                // repopulate vehicle and drop-down lists
                var stateRepo = StateRepositoryFactory.GetRepository();
                var purchaseTypeRepo = PurchaseTypeRepositoryFactory.GetRepository();
                var vehicleRepo = VehicleRepositoryFactory.GetRepository();

                model.States = new SelectList(stateRepo.GetAll(), "StateId", "StateId");
                model.PurchaseTypes = new SelectList(purchaseTypeRepo.GetAll(), "PurchaseTypeId", "PurchaseTypeName");
                model.VehicleDetails = vehicleRepo.GetDetails(id);

                return View("Purchase", model); // return with filled-out data
            }
        }
    }
}