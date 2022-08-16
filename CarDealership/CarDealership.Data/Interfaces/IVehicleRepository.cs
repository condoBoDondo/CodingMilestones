using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Interfaces
{
    public interface IVehicleRepository
    {
        IEnumerable<Vehicle> GetAll();
        Vehicle GetById(int vehicleId);
        int GetCurrentId(); // get identity column's current ID (used for image naming)
        IEnumerable<VehicleFeatured> GetFeatured(); // get list of vehicles formatted for home page
        IEnumerable<VehicleReport> GetReports(string mode); // get list of vehicles formatted for admin reports page
        IEnumerable<VehicleListing> Search(VehicleSearchParameters parameters, string mode); // get list of vehicles formatted for search page
        VehicleDetails GetDetails(int vehicleId); // get single detail-heavy vehicle for individual page
        void Insert(Vehicle vehicle);
        void Update(Vehicle vehicle);
        void Delete(int vehicleId);
    }
}
