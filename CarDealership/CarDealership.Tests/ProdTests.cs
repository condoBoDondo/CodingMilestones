using CarDealership.Data.Factories;
using CarDealership.Data.Prod;
using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Tests
{
    // requires separate app.config file

    [TestFixture]
    public class ProdTests
    {
        [SetUp]
        public void Init()
        {
            // reset database before each test
            // note: conn. string here is from app.config in CarDealership.Tests,
            //   not from web.config in CarDealership.UI
            using(var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var cmd = new SqlCommand();
                cmd.CommandText = "DbReset";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Connection = cn;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void CanGetBodyStyles()
        {
            var repo = new BodyStyleRepositoryProd(); // get prod repo
            var styles = repo.GetAll().ToList(); // get list of all body styles

            Assert.AreEqual(4, styles.Count()); // get expected count
            Assert.AreEqual("SUV", styles[1].BodyStyleName); // get expected item at index
        }

        [Test]
        public void CanGetExteriors()
        {
            var repo = new ExteriorRepositoryProd();
            var exteriors = repo.GetAllWithColorNames().ToList();

            Assert.AreEqual(5, exteriors.Count());
            Assert.AreEqual(6, exteriors[4].ColorId);
            Assert.AreEqual("Electric Blue", exteriors[4].ColorName);
        }

        [Test]
        public void CanGetInteriors()
        {
            var repo = new InteriorRepositoryProd();
            var interiors = repo.GetAllWithColorNames().ToList();

            Assert.AreEqual(5, interiors.Count());
            Assert.AreEqual(7, interiors[3].ColorId);
            Assert.AreEqual("Mahogany", interiors[3].ColorName);
        }

        [Test]
        public void CanGetMakes()
        {
            var repo = new MakeRepositoryProd();
            var makes = repo.GetAll().ToList();

            Assert.AreEqual(5, makes.Count());
            Assert.AreEqual("Chevrolet", makes[1].MakeName);
        }

        [Test]
        public void CanGetMakeReports()
        {
            var repo = new MakeRepositoryProd();
            var makes = repo.GetReports().ToList();

            Assert.AreEqual(5, makes.Count());
            Assert.AreEqual("test", makes[3].AddedBy);
        }

        [Test]
        public void CanInsertMake()
        {
            var repo = new MakeRepositoryProd();
            Make make = new Make
            {
                MakeName = "Test Make",
                AddedBy = "00000000-0000-0000-0000-000000000000"
            };

            repo.Insert(make);

            Assert.AreEqual(6, repo.GetAll().Count());
            Assert.AreEqual(6, make.MakeId);
        }

        [Test]
        public void CanGetModels()
        {
            var repo = new ModelRepositoryProd();
            var models = repo.GetAll().ToList();

            Assert.AreEqual(10, models.Count());
            Assert.AreEqual("Malibu", models[3].ModelName);
        }

        [Test]
        public void CanGetModelsByMakeId()
        {
            var repo = new ModelRepositoryProd();
            var models = repo.GetByMakeId(5).ToList();

            Assert.AreEqual(2, models.Count());
            Assert.AreEqual("Outback", models[1].ModelName);
        }

        [Test]
        public void CanGetModelReports()
        {
            var repo = new ModelRepositoryProd();
            var models = repo.GetReports().ToList();

            Assert.AreEqual(10, models.Count());
            Assert.AreEqual("test", models[3].AddedBy);
        }

        [Test]
        public void CanInsertModel()
        {
            var repo = new ModelRepositoryProd();
            Model model = new Model
            {
                MakeId = 3,
                ModelName = "Test Ford Model",
                AddedBy = "00000000-0000-0000-0000-000000000000"
            };

            repo.Insert(model);

            Assert.AreEqual(11, repo.GetAll().Count());
            Assert.AreEqual(11, model.ModelId);
        }

        [Test]
        public void CanGetPurchaseTypes()
        {
            var repo = new PurchaseTypeRepositoryProd();
            var types = repo.GetAll().ToList();

            Assert.AreEqual(3, types.Count());
            Assert.AreEqual("Cash", types[1].PurchaseTypeName);
        }

        [Test]
        public void CanGetSaleReports()
        {
            var repo = new SaleRepositoryProd();
            var search = new SaleReportSearchParameters();
            var saleReports = repo.GetReports(search).ToList();

            Assert.AreEqual(2, saleReports.Count());
        }

        [Test]
        public void CanGetSaleReportsWithFilter()
        {
            var repo = new SaleRepositoryProd();
            var search = new SaleReportSearchParameters();
            search.Salesperson = "6af6dd06-8777-4802-a72e-40a6dbdfa269";
            var saleReports = repo.GetReports(search).ToList();

            Assert.AreEqual(1, saleReports.Count());
        }

        [Test]
        public void CanPurchaseSale()
        {
            var repo = new SaleRepositoryProd();
            Sale sale = new Sale
            {
                CustomerName = "Test Customer",
                Phone = "123-123-1234",
                Email = "customer@test.com",
                Address1 = "123 Test Rd",
                City = "Testville",
                StateId = "MN",
                Zip = "12345",
                VehicleId = 2,
                PurchasePrice = 4400.00m,
                PurchaseTypeId = 2,
                Salesperson = "00000000-0000-0000-0000-000000000000"
            };

            repo.Purchase(sale);

            Assert.AreEqual(7, repo.GetAll().Count());

            var vehicleRepo = new VehicleRepositoryProd();
            var vehicle = vehicleRepo.GetById(2);

            // also checking vehicle for updated listing status
            Assert.AreEqual(3, vehicle.ListingStatusId);
        }

        [Test]
        public void CanGetSpecials()
        {
            var repo = new SpecialRepositoryProd();
            var specials = repo.GetAll().ToList();

            Assert.AreEqual(3, specials.Count());
            Assert.AreEqual("Big Big Big!", specials[1].Title);
        }

        [Test]
        public void CanInsertSpecial()
        {
            var repo = new SpecialRepositoryProd();
            Special special = new Special
            {
                Title = "Test Sale!",
                Description = "This is a description for the test sale."
            };

            repo.Insert(special);

            Assert.AreEqual(4, repo.GetAll().Count());
            Assert.AreEqual(4, special.SpecialId);
        }

        [Test]
        public void CanDeleteSpecial()
        {
            var repo = new SpecialRepositoryProd();
            repo.Delete(2);
            var newRepo = repo.GetAll().ToList();

            Assert.AreEqual(2, repo.GetAll().Count());
            Assert.AreEqual("Huge Sale!", newRepo[1].Title);
        }

        [Test]
        public void CanGetStates()
        {
            var repo = new StateRepositoryProd();
            var states = repo.GetAll().ToList();

            Assert.AreEqual(5, states.Count());
        }

        [Test]
        public void CanGetTransmissions()
        {
            var repo = new TransmissionRepositoryProd();
            var ts = repo.GetAll().ToList();

            Assert.AreEqual(2, ts.Count());
            Assert.AreEqual("Automatic", ts[0].TransmissionName);
        }

        [Test]
        public void CanGetFeaturedVehicles()
        {
            var repo = new VehicleRepositoryProd();
            var vehicles = repo.GetFeatured().ToList();

            Assert.AreEqual(6, vehicles.Count());
            Assert.AreEqual("Forester", vehicles[5].ModelName);
        }

        [Test]
        public void CanGetVehicleReports()
        {
            var repo = new VehicleRepositoryProd();
            var reports = repo.GetReports("new").ToList();

            Assert.AreEqual(2, reports.Count());

            reports = repo.GetReports("used").ToList();

            Assert.AreEqual(3, reports.Count());
        }

        [Test]
        public void CanSearchVehiclesByNew()
        {
            var repo = new VehicleRepositoryProd();
            var vehicles = repo.Search(new VehicleSearchParameters { }, "new");

            Assert.AreEqual(3, vehicles.Count());
        }

        [Test]
        public void CanSearchVehiclesByUsed()
        {
            var repo = new VehicleRepositoryProd();
            var vehicles = repo.Search(new VehicleSearchParameters { }, "used");

            Assert.AreEqual(3, vehicles.Count());
        }

        [Test]
        public void CanSearchVehiclesBySales()
        {
            var repo = new VehicleRepositoryProd();
            var vehicles = repo.Search(new VehicleSearchParameters { }, "sales");

            Assert.AreEqual(4, vehicles.Count());
        }

        [Test]
        public void CanSearchVehiclesByAdmin()
        {
            var repo = new VehicleRepositoryProd();
            var vehicles = repo.Search(new VehicleSearchParameters { }, "admin");

            Assert.AreEqual(6, vehicles.Count());
        }

        [Test]
        public void CanSearchVehiclesWithFilter()
        {
            var repo = new VehicleRepositoryProd();
            var input = new VehicleSearchParameters();

            input.MinYear = 2009;
            var vehicles = repo.Search(input, "used");
            Assert.AreEqual(2, vehicles.Count());

            input.MinYear = null;
            input.MaxYear = 2010;
            vehicles = repo.Search(input, "admin");
            Assert.AreEqual(5, vehicles.Count());

            input.MaxYear = null;
            input.Input = "Bronco";
            vehicles = repo.Search(input, "sales");
            Assert.AreEqual(1, vehicles.Count());

            input.Input = "2009";
            vehicles = repo.Search(input, "new");
            Assert.AreEqual(2, vehicles.Count());
        }

        [Test]
        public void CanGetVehicleDetails()
        {
            var repo = new VehicleRepositoryProd();
            var vehicle = repo.GetDetails(4);

            Assert.AreEqual("Palisade", vehicle.ModelName);
            Assert.AreEqual(25000.00m, vehicle.MSRP);
        }

        [Test]
        public void CanInsertVehicle()
        {
            var repo = new VehicleRepositoryProd();
            Vehicle vehicle = new Vehicle
            {
                MakeId = 4,
                ModelId = 8,
                BodyStyleId = 1,
                Year = 2020,
                TransmissionId = 1,
                ExteriorId = 4,
                InteriorId = 3,
                Mileage = 2500,
                VIN = "1KFD9LA05J392LNO8",
                MSRP = 22000.00m,
                SalePrice = 19500.00m,
                Description = "Here's a test description.",
                ImageFileName = "placeholder.png"
            };

            repo.Insert(vehicle);

            Assert.AreEqual(9, repo.GetAll().Count());
            Assert.AreEqual(9, vehicle.VehicleId);
        }

        [Test]
        public void CanUpdateVehicle()
        {
            var repo = new VehicleRepositoryProd();
            Vehicle vehicle = repo.GetAll().ToList()[2];

            vehicle.Description = "Updated description!";

            repo.Update(vehicle);

            var updatedRepo = repo.GetAll().ToList();

            Assert.AreEqual(8, updatedRepo.Count());
            Assert.AreEqual("Updated description!", updatedRepo[2].Description);
        }

        [Test]
        public void CanDeleteVehicle()
        {
            var repo = new VehicleRepositoryProd();
            repo.Delete(3);

            Assert.AreEqual(7, repo.GetAll().Count());
        }
    }
}
