using CarDealership.Data.Factories;
using CarDealership.UI.Models;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarDealership.Models.Queries;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace CarDealership.UI.Controllers
{
    [Authorize(Roles = "admin")] // admin only
    public class AdminController : Controller
    {
        // all administrative links in index
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Makes()
        {
            var model = new MakeAddViewModel();
            model.MakeReports = MakeRepositoryFactory.GetRepository().GetReports();
            model.Make = new Make();

            return View(model);
        }

        public ActionResult MakeAdd(MakeAddViewModel model)
        {
            if (ModelState.IsValid) // make sure entered data was valid
            {
                var repo = MakeRepositoryFactory.GetRepository();

                try
                {
                    repo.Insert(model.Make); // add model to repo
                    return RedirectToAction("Makes");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else // if not valid
            {
                // need to repopulate model's properties
                model.MakeReports = MakeRepositoryFactory.GetRepository().GetReports();

                return View("Makes", model); // return model with filled-out data
            }
        }

        public ActionResult Models()
        {
            var model = new ModelAddViewModel();

            var makeRepo = MakeRepositoryFactory.GetRepository();

            model.ModelReports = ModelRepositoryFactory.GetRepository().GetReports(); // get all models
            model.Makes = new SelectList(makeRepo.GetAll(), "MakeId", "MakeName"); // populate select list with makes
            model.ModelModel = new Model();

            return View(model);
        }

        public ActionResult ModelAdd(ModelAddViewModel model)
        {
            if (ModelState.IsValid) // make sure entered data was valid
            {
                var repo = ModelRepositoryFactory.GetRepository();

                try
                {
                    repo.Insert(model.ModelModel); // add model to repo
                    return RedirectToAction("Models");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else // if not valid
            {
                // need to repopulate model's properties
                var makeRepo = MakeRepositoryFactory.GetRepository();

                model.ModelReports = ModelRepositoryFactory.GetRepository().GetReports();
                model.Makes = new SelectList(makeRepo.GetAll(), "MakeId", "MakeName");

                return View("Models", model); // return model with filled-out data
            }
        }

        public ActionResult Specials()
        {
            var model = new SpecialAddViewModel();
            model.Specials = SpecialRepositoryFactory.GetRepository().GetAll();
            model.Special = new Special();

            return View(model);
        }

        [HttpPost]
        public ActionResult SpecialAdd(SpecialAddViewModel model)
        {
            if (ModelState.IsValid) // make sure entered data was valid
            {
                var repo = SpecialRepositoryFactory.GetRepository();

                try
                {
                    repo.Insert(model.Special); // add model to repo
                    return RedirectToAction("Specials");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else // if not valid
            {
                // need to repopulate model's properties
                model.Specials = SpecialRepositoryFactory.GetRepository().GetAll();

                return View("Specials", model); // return model with filled-out data
            }
        }

        [HttpPost]
        public ActionResult SpecialDelete(int id)
        {
            var repo = SpecialRepositoryFactory.GetRepository();
            repo.Delete(id);

            return RedirectToAction("Specials");
        }

        public ActionResult Vehicles(VehicleSearchParameters parameters)
        {
            var repo = VehicleRepositoryFactory.GetRepository();
            var search = repo.Search(parameters, "admin");

            return View(search);
        }

        public ActionResult VehicleAdd()
        {
            var model = new VehicleAddViewModel();

            // get all repos required for model
            var bodyStyleRepo = BodyStyleRepositoryFactory.GetRepository();
            var exteriorRepo = ExteriorRepositoryFactory.GetRepository();
            var interiorRepo = InteriorRepositoryFactory.GetRepository();
            var makeRepo = MakeRepositoryFactory.GetRepository();
            var modelRepo = ModelRepositoryFactory.GetRepository();
            var transmissionRepo = TransmissionRepositoryFactory.GetRepository();

            // put repos as select lists
            model.BodyStyles = new SelectList(bodyStyleRepo.GetAll(), "BodyStyleId", "BodyStyleName");
            model.Exteriors = new SelectList(exteriorRepo.GetAllWithColorNames(), "ExteriorId", "ColorName");
            model.Interiors = new SelectList(interiorRepo.GetAllWithColorNames(), "InteriorId", "ColorName");
            model.Makes = new SelectList(makeRepo.GetAll(), "MakeId", "MakeName");
            model.Models = new SelectList(modelRepo.GetAll(), "ModelId", "ModelName");
            model.Transmissions = new SelectList(transmissionRepo.GetAll(), "TransmissionId", "TransmissionName");

            model.Vehicle = new Vehicle();

            return View(model);
        }

        [HttpPost]
        public ActionResult VehicleAdd(VehicleAddViewModel model)
        {
            ViewBag.SelectedModelId = model.Vehicle.ModelId;

            if (ModelState.IsValid)
            {
                // repo to hold new vehicle
                var vehicleRepo = VehicleRepositoryFactory.GetRepository();
                
                // get vehicle-to-be-added's new id to append to image name
                // necessary as the id isn't acquired until after the vehicle is inserted into repo
                int newVehicleId = vehicleRepo.GetCurrentId() + 1;

                try
                {
                    // image upload
                    if (model.ImageUpload != null)
                    {
                        var savePath = Server.MapPath("~/Images"); // get path to server

                        // image file should be renamed as: "inventory-x-y.ext"
                        // x is vehicleId, y is VIN
                        var fileName = "inventory-" + newVehicleId + "-" + model.Vehicle.VIN;
                        var fileExt = Path.GetExtension(model.ImageUpload.FileName); // get file extension
                        var filePath = Path.Combine(savePath, fileName + fileExt); // get complete file path

                        int counter = 1;

                        // if file exists, use counter to find unused name
                        // need to specify System.IO.File; separate File class exists in ASP.NET (namespace collision)
                        while (System.IO.File.Exists(filePath))
                        {
                            filePath = Path.Combine(savePath, fileName + "(" + counter.ToString() + ")" + fileExt);
                            counter++;
                        }

                        model.ImageUpload.SaveAs(filePath); // save file as this name/path
                        model.Vehicle.ImageFileName = Path.GetFileName(filePath); // set vehicle's image name
                    }

                    vehicleRepo.Insert(model.Vehicle); // add vehicle to repo

                    return RedirectToAction("VehicleEdit", new { id = model.Vehicle.VehicleId });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                // repopulate drop-down lists upon failure
                var bodyStyleRepo = BodyStyleRepositoryFactory.GetRepository();
                var exteriorRepo = ExteriorRepositoryFactory.GetRepository();
                var interiorRepo = InteriorRepositoryFactory.GetRepository();
                var makeRepo = MakeRepositoryFactory.GetRepository();
                var modelRepo = ModelRepositoryFactory.GetRepository();
                var transmissionRepo = TransmissionRepositoryFactory.GetRepository();

                model.BodyStyles = new SelectList(bodyStyleRepo.GetAll(), "BodyStyleId", "BodyStyleName");
                model.Exteriors = new SelectList(exteriorRepo.GetAllWithColorNames(), "ExteriorId", "ColorName");
                model.Interiors = new SelectList(interiorRepo.GetAllWithColorNames(), "InteriorId", "ColorName");
                model.Makes = new SelectList(makeRepo.GetAll(), "MakeId", "MakeName");
                model.Models = new SelectList(modelRepo.GetAll(), "ModelId", "ModelName");
                model.Transmissions = new SelectList(transmissionRepo.GetAll(), "TransmissionId", "TransmissionName");

                return View(model);
            }
        }

        public ActionResult VehicleEdit(int id)
        {
            var model = new VehicleEditViewModel();

            // get all repos required for model
            var bodyStyleRepo = BodyStyleRepositoryFactory.GetRepository();
            var exteriorRepo = ExteriorRepositoryFactory.GetRepository();
            var interiorRepo = InteriorRepositoryFactory.GetRepository();
            var makeRepo = MakeRepositoryFactory.GetRepository();
            var modelRepo = ModelRepositoryFactory.GetRepository();
            var transmissionRepo = TransmissionRepositoryFactory.GetRepository();
            var vehicleRepo = VehicleRepositoryFactory.GetRepository(); // need specified vehicle

            // put repos as select lists
            model.BodyStyles = new SelectList(bodyStyleRepo.GetAll(), "BodyStyleId", "BodyStyleName");
            model.Exteriors = new SelectList(exteriorRepo.GetAllWithColorNames(), "ExteriorId", "ColorName");
            model.Interiors = new SelectList(interiorRepo.GetAllWithColorNames(), "InteriorId", "ColorName");
            model.Makes = new SelectList(makeRepo.GetAll(), "MakeId", "MakeName");
            model.Models = new SelectList(modelRepo.GetAll(), "ModelId", "ModelName");
            model.Transmissions = new SelectList(transmissionRepo.GetAll(), "TransmissionId", "TransmissionName");

            model.Vehicle = vehicleRepo.GetById(id); // getting existing instead of making new
            model.IsFeatured = model.Vehicle.ListingStatusId == 2 ? true : false; // set featured button based on listing status

            return View(model);
        }

        [HttpPost]
        public ActionResult VehicleEdit(VehicleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // set featured checkbox to correspond with ListingStatusId 2
                model.Vehicle.ListingStatusId = model.IsFeatured ? 2 : 1;

                // repo to hold newly-edited vehicle
                var vehicleRepo = VehicleRepositoryFactory.GetRepository();

                try
                {
                    // get old vehicle to check image
                    var oldVehicle = vehicleRepo.GetById(model.Vehicle.VehicleId);

                    // image upload (optional)
                    if (model.ImageUpload != null)
                    {
                        var savePath = Server.MapPath("~/Images"); // get path to server

                        // image file should be renamed as: "inventory-x-y.ext"
                        // x is vehicleId, y is VIN
                        var fileName = "inventory-" + model.Vehicle.VehicleId + "-" + model.Vehicle.VIN;
                        var fileExt = Path.GetExtension(model.ImageUpload.FileName); // get file extension
                        var filePath = Path.Combine(savePath, fileName + fileExt); // get complete file path

                        int counter = 1;

                        // if file exists, use counter to find unused name
                        // need to specify System.IO.File; separate File class exists in ASP.NET (namespace collision)
                        while (System.IO.File.Exists(filePath))
                        {
                            filePath = Path.Combine(savePath, fileName + "(" + counter.ToString() + ")" + fileExt);
                            counter++;
                        }

                        model.ImageUpload.SaveAs(filePath); // save file as this name/path
                        model.Vehicle.ImageFileName = Path.GetFileName(filePath); // set vehicle's image name

                        // delete old file
                        var oldPath = Path.Combine(savePath, oldVehicle.ImageFileName);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                    else // use existing image
                        model.Vehicle.ImageFileName = oldVehicle.ImageFileName;

                    vehicleRepo.Update(model.Vehicle); // update vehicle

                    return RedirectToAction("VehicleEdit", new { id = model.Vehicle.VehicleId });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                // repopulate drop-down lists upon failure
                var bodyStyleRepo = BodyStyleRepositoryFactory.GetRepository();
                var exteriorRepo = ExteriorRepositoryFactory.GetRepository();
                var interiorRepo = InteriorRepositoryFactory.GetRepository();
                var makeRepo = MakeRepositoryFactory.GetRepository();
                var modelRepo = ModelRepositoryFactory.GetRepository();
                var transmissionRepo = TransmissionRepositoryFactory.GetRepository();

                model.BodyStyles = new SelectList(bodyStyleRepo.GetAll(), "BodyStyleId", "BodyStyleName");
                model.Exteriors = new SelectList(exteriorRepo.GetAllWithColorNames(), "ExteriorId", "ColorName");
                model.Interiors = new SelectList(interiorRepo.GetAllWithColorNames(), "InteriorId", "ColorName");
                model.Makes = new SelectList(makeRepo.GetAll(), "MakeId", "MakeName");
                model.Models = new SelectList(modelRepo.GetAll(), "ModelId", "ModelName");
                model.Transmissions = new SelectList(transmissionRepo.GetAll(), "TransmissionId", "TransmissionName");

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult VehicleDelete(int id)
        {
            var repo = VehicleRepositoryFactory.GetRepository();
            repo.Delete(id);

            return RedirectToAction("Vehicles");
        }

        public ActionResult ReportInventory()
        {
            var model = new VehicleReportViewModel();
            var repo = VehicleRepositoryFactory.GetRepository();

            model.NewVehicleReports = repo.GetReports("new");
            model.UsedVehicleReports = repo.GetReports("used");

            return View(model);
        }

        public ActionResult ReportSale(SaleReportSearchParameters parameters)
        {
            // get the db's context and user manager to get asp.net users from database
            var dbContext = new CarDealershipDbContext();
            var userMgr = new UserManager<AppUser>(new UserStore<AppUser>(dbContext));

            // make a SelectList from users, using Id as values and Email as text
            ViewBag.Users = new SelectList(userMgr.Users, nameof(AppUser.Id), nameof(AppUser.Email));

            /* testing things out
            using(var context = new CarDealershipDbContext())
            {
                //ViewBag.UserEmails = context.Users.Select(u => u.Email).ToList();
                //ViewBag.UserIds = context.Users.Select(u => u.Id).ToList();

                //ViewBag.Users = new SelectList(context.Users, nameof(AppUser.Id), nameof(AppUser.Email));
            }
            */

            var model = new SaleReportViewModel();
            var salesRepo = SaleRepositoryFactory.GetRepository();

            model.SaleReports = salesRepo.GetReports(parameters);

            return View(model);
        }

        public ActionResult Users()
        {
            // get all users
            var model = UserRepositoryFactory.GetRepository().GetAll();

            return View(model);
        }

        public ActionResult UserAdd()
        {
            var model = new UserAddViewModel();
            var roleRepo = UserRoleRepositoryFactory.GetRepository();

            // populate roles dropdown
            // user manager's AddToRole requires name of role, not Id
            model.Roles = new SelectList(roleRepo.GetAll(), "Name", "Name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAdd(UserAddViewModel model)
        {
            if(ModelState.IsValid)
            {
                // get the db's context and user manager to get asp.net users from database
                var dbContext = new CarDealershipDbContext();
                var userMgr = new UserManager<AppUser>(new UserStore<AppUser>(dbContext));

                // create new user
                var newUser = new AppUser()
                { // UserName and Email are the same; no reason to separate them for this
                    UserName = model.User.Email,
                    FirstName = model.User.FirstName,
                    LastName = model.User.LastName,
                    Email = model.User.Email
                };

                // create user login with input password
                userMgr.Create(newUser, model.Password);

                // add user to given role
                userMgr.AddToRole(newUser.Id, model.User.RoleName);

                return RedirectToAction("Users");
            }
            else
            {
                // repopulate dropdown
                var roleRepo = UserRoleRepositoryFactory.GetRepository();
                model.Roles = new SelectList(roleRepo.GetAll(), "Name", "Name");

                return View(model);
            }
        }

        public ActionResult UserEdit(string id)
        {
            var model = new UserEditViewModel();
            var userRepo = UserRepositoryFactory.GetRepository();
            var roleRepo = UserRoleRepositoryFactory.GetRepository();

            model.Roles = new SelectList(roleRepo.GetAll(), "Name", "Name");
            model.User = userRepo.GetById(id); // asp.net user ids are long strings

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserEdit(UserEditViewModel model)
        {
            // async method for easy info changing
            if (ModelState.IsValid)
            {
                // get the db's context, user manager, repo to get asp.net users from database
                var dbContext = new CarDealershipDbContext();
                var userMgr = new UserManager<AppUser>(new UserStore<AppUser>(dbContext));
                var userRepo = UserRepositoryFactory.GetRepository();

                // testing changing password; gets current and new password
                var result = await userMgr.ChangePasswordAsync(model.User.Id, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    // current password didn't match
                    ModelState.AddModelError("", "Current Password was incorrect.");
                    return View(model);
                }

                // get all roles as a string array
                //var allRoles = UserRoleRepositoryFactory.GetRepository().GetAll().Select(r => r.Name).ToArray();

                // get all roles the user is in as a string array
                string[] allRoles = userMgr.GetRoles(model.User.Id).ToArray();

                userMgr.RemoveFromRoles(model.User.Id, allRoles);
                userMgr.AddToRole(model.User.Id, model.User.RoleName);

                // change first name, last name, username, email
                userRepo.Update(model.User);

                return RedirectToAction("Users");
            }
            else
            {
                // repopulate dropdown
                var roleRepo = UserRoleRepositoryFactory.GetRepository();
                model.Roles = new SelectList(roleRepo.GetAll(), "Name", "Name");

                return View(model);
            }
        }
    }
}