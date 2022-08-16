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
    public class SaleRepositoryProd : ISaleRepository
    {
        public IEnumerable<Sale> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<Sale>("SaleSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<SaleReport> GetReports(SaleReportSearchParameters search)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@Salesperson", search.Salesperson);
                p.Add("@MinDate", search.MinDate);
                p.Add("@MaxDate", search.MaxDate);

                return cn.Query<SaleReport>("SaleAggregate", p,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public void Purchase(Sale sale)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@SaleId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@CustomerName", sale.CustomerName);
                p.Add("@Phone", sale.Phone);
                p.Add("@Email", sale.Email);
                p.Add("@Address1", sale.Address1);
                p.Add("@Address2", sale.Address2);
                p.Add("@City", sale.City);
                p.Add("@StateId", sale.StateId);
                p.Add("@Zip", sale.Zip);
                p.Add("@VehicleId", sale.VehicleId);
                p.Add("@PurchasePrice", sale.PurchasePrice);
                p.Add("@PurchaseTypeId", sale.PurchaseTypeId);
                p.Add("@Salesperson", sale.Salesperson);

                cn.Execute("SalePurchase", p, commandType: CommandType.StoredProcedure);

                sale.SaleId = p.Get<int>("@SaleId");
            }
        }
    }
}
