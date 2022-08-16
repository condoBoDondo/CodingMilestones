using CarDealership.Data.Interfaces;
using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Prod
{
    public class VehicleRepositoryProd : IVehicleRepository
    {
        public IEnumerable<Vehicle> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<Vehicle>("VehicleSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public Vehicle GetById(int vehicleId)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@VehicleId", vehicleId);

                // FirstOrDefault as we're only returning one item
                return cn.Query<Vehicle>("VehicleSelectById", p,
                    commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public IEnumerable<VehicleFeatured> GetFeatured()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<VehicleFeatured>("VehicleSelectFeatured",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<VehicleReport> GetReports(string mode)
        {
            // building SQL statement here due to the amount of variables
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();
                string query = "";

                // query counts vehicles still in stock (divided into new/used via mode) and gets total value
                // sorts by year -> make -> model

                // initial bulk of query
                // indented for readability; space put after each line to not run statements together
                query +=
                    "SELECT [Year], ma.MakeName, mo.ModelName, COUNT([Year]) AS [Count], SUM(MSRP) AS StockValue " +
                    "FROM Vehicle v " +
                        "INNER JOIN Make ma ON v.MakeId = ma.MakeId " +
                        "INNER JOIN Model mo ON v.ModelId = mo.ModelId " +
                    "WHERE ListingStatusId IN (1,2) "; // is not or is featured

                // ANDs based on mode
                switch(mode)
                {
                    case "new": // new: 0-1000 mileage
                        query += "AND Mileage BETWEEN 0 AND 1000 ";
                        break;
                    case "used": // used: > 1000 mileage
                        query += "AND Mileage > 1000 ";
                        break;
                    default: // default: select nothing
                        query += "AND 1=0 ";
                        break;
                }

                // close off query
                query +=
                    "GROUP BY [Year], ma.MakeName, mo.ModelName " +
                    "ORDER BY [Year] DESC, ma.MakeName, mo.ModelName";

                //return cn.Query<VehicleReport>("VehicleAggregate" + mode,
                //    commandType: CommandType.StoredProcedure);

                // return results
                return cn.Query<VehicleReport>(query);
            }
        }

        public IEnumerable<VehicleListing> Search(VehicleSearchParameters parameters, string mode)
        {
            // building SQL statement in here due to the amount of variables
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();
                string query = "";

                // initial bulk of query
                query +=
                    "SELECT TOP 20 " +
                        "VehicleId, ma.MakeName, mo.ModelName, b.BodyStyleName, [Year], " +
                        "t.TransmissionName, ce.ColorName AS ExteriorName, ci.ColorName AS InteriorName, " +
                        "Mileage, VIN, MSRP, SalePrice, ImageFileName, ListingStatusId " +
                    "FROM Vehicle v " +
                        "INNER JOIN Make ma ON v.MakeId = ma.MakeId " +
                        "INNER JOIN Model mo ON v.ModelId = mo.ModelId " +
                        "INNER JOIN BodyStyle b ON v.BodyStyleId = b.BodyStyleId " +
                        "INNER JOIN Transmission t ON v.TransmissionId = t.TransmissionId " +
                        "INNER JOIN Exterior e ON v.ExteriorId = e.ExteriorId " +
                        "INNER JOIN Interior i ON v.InteriorId = i.InteriorId " +
                        "INNER JOIN Color ce ON e.ColorId = ce.ColorId " +
                        "INNER JOIN Color ci ON i.ColorId = ci.ColorId ";

                // determine which listing statuses and mileages will be chosen
                switch (mode) 
                {
                    case "new": // new: featured/purchased, 0-1000 mileage
                        query +=
                            "WHERE ListingStatusId IN (2,3) " +
                            "AND Mileage BETWEEN 0 AND 1000 ";
                        break;
                    case "used": // used: featured/purchased, > 1000 mileage
                        query +=
                            "WHERE ListingStatusId IN (2,3) " +
                            "AND Mileage > 1000 ";
                        break;
                    case "sales": // sales: featured, any mileage
                        query +=
                            "WHERE ListingStatusId = 2 ";
                        break;
                    case "admin": // admin: not featured/featured, any mileage
                        query +=
                            "WHERE ListingStatusId IN (1,2) ";
                        break;
                    default: // default: select nothing
                        query +=
                            "WHERE 1=0 ";
                        break;
                }

                // get search field input
                // try converting input to a year value first
                query += parameters.Input == null ? "" :
                    "AND ([Year] = TRY_CONVERT(int, @Input) " +
                        "OR ma.MakeName LIKE '%' + @Input + '%' " +
                        "OR mo.ModelName LIKE '%' + @Input + '%') ";

                // get min/max price
                // not sure why I was so insistent on using BETWEEN
                query += !(parameters.MinPrice.HasValue || parameters.MaxPrice.HasValue) ? "" :
                    "AND SalePrice BETWEEN ISNULL(@MinPrice, 0.00) AND ISNULL(@MaxPrice, 9999999.99) ";

                // get min/max year
                query += !(parameters.MinYear.HasValue || parameters.MaxYear.HasValue) ? "" :
                    "AND [Year] BETWEEN ISNULL(@MinYear, 1) AND ISNULL(@MaxYear, 9999) ";

                // close off query
                query +=
                    "ORDER BY MSRP DESC";

                // get input params
                DynamicParameters p = new DynamicParameters();
                p.Add("@Input", parameters.Input);
                p.Add("@MinPrice", parameters.MinPrice);
                p.Add("@MaxPrice", parameters.MaxPrice);
                p.Add("@MinYear", parameters.MinYear);
                p.Add("@MaxYear", parameters.MaxYear);

                // return results
                return cn.Query<VehicleListing>(query, p);
            }
        }

        public VehicleDetails GetDetails(int vehicleId)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@VehicleId", vehicleId);

                // FirstOrDefault as we're only returning one item
                return cn.Query<VehicleDetails>("VehicleSelectDetails", p,
                    commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public void Insert(Vehicle vehicle)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@VehicleId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@MakeId", vehicle.MakeId);
                p.Add("@ModelId", vehicle.ModelId);
                p.Add("@BodyStyleId", vehicle.BodyStyleId);
                p.Add("@Year", vehicle.Year);
                p.Add("@TransmissionId", vehicle.TransmissionId);
                p.Add("@ExteriorId", vehicle.ExteriorId);
                p.Add("@InteriorId", vehicle.InteriorId);
                p.Add("@Mileage", vehicle.Mileage);
                p.Add("@VIN", vehicle.VIN);
                p.Add("@MSRP", vehicle.MSRP);
                p.Add("@SalePrice", vehicle.SalePrice);
                p.Add("@Description", vehicle.Description);
                p.Add("@ImageFileName", vehicle.ImageFileName);

                cn.Execute("VehicleInsert", p, commandType: CommandType.StoredProcedure);

                vehicle.VehicleId = p.Get<int>("@VehicleId"); // returning new VehicleId
            }
        }

        public void Update(Vehicle vehicle)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@VehicleId", vehicle.VehicleId);
                p.Add("@MakeId", vehicle.MakeId);
                p.Add("@ModelId", vehicle.ModelId);
                p.Add("@BodyStyleId", vehicle.BodyStyleId);
                p.Add("@Year", vehicle.Year);
                p.Add("@TransmissionId", vehicle.TransmissionId);
                p.Add("@ExteriorId", vehicle.ExteriorId);
                p.Add("@InteriorId", vehicle.InteriorId);
                p.Add("@Mileage", vehicle.Mileage);
                p.Add("@VIN", vehicle.VIN);
                p.Add("@MSRP", vehicle.MSRP);
                p.Add("@SalePrice", vehicle.SalePrice);
                p.Add("@Description", vehicle.Description);
                p.Add("@ImageFileName", vehicle.ImageFileName);
                p.Add("@ListingStatusId", vehicle.ListingStatusId);

                cn.Execute("VehicleUpdate", p, commandType: CommandType.StoredProcedure);
            }
        }

        public void Delete(int vehicleId)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@VehicleId", vehicleId);

                cn.Execute("VehicleDelete", p, commandType: CommandType.StoredProcedure);
            }
        }

        public int GetCurrentId()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                string query = "SELECT IDENT_CURRENT('Vehicle')";

                return cn.Query<int>(query).FirstOrDefault();
            }
        }
    }
}
