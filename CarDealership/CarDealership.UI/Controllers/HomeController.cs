using CarDealership.Data.Factories;
using CarDealership.Models.Queries;
using CarDealership.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarDealership.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // get special and vehicle repos to display on main page
            var model = new IndexViewModel(); // this should probably be replaced by a viewmodel
            model.Specials = SpecialRepositoryFactory.GetRepository().GetAll();
            model.Vehicles = VehicleRepositoryFactory.GetRepository().GetFeatured();

            return View(model);
        }

        public ActionResult Specials()
        {
            var model = SpecialRepositoryFactory.GetRepository().GetAll();

            return View(model);
        }

        public ActionResult Contact(string regarding)
        {
            var model = new ContactViewModel();
            model.Contact = new CarDealership.Models.Tables.Contact();

            // get what this is regarding, if possible
            if (!string.IsNullOrEmpty(regarding))
                model.Contact.Regarding = regarding;

            return View(model);
        }

        [HttpPost]
        public ActionResult ContactAdd(ContactViewModel model)
        {
            if (ModelState.IsValid) // make sure entered data was valid
            {
                var contactRepo = ContactRepositoryFactory.GetRepository();

                try
                {
                    contactRepo.Insert(model.Contact); // add model to repo
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else // if not valid
            {
                return View("Contact", model); // return model with filled-out data
            }
        }
    }
}