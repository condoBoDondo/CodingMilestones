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
    public class MakeRepositoryProd : IMakeRepository
    {
        public IEnumerable<Make> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<Make>("MakeSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<MakeReport> GetReports()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<MakeReport>("MakeSelectAllReport",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public void Insert(Make make)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                // set up output and input params
                DynamicParameters p = new DynamicParameters();
                p.Add("@MakeId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@MakeName", make.MakeName);
                p.Add("@AddedBy", make.AddedBy);

                // execute command, with these params, of this type
                cn.Execute("MakeInsert", p, commandType: CommandType.StoredProcedure);

                make.MakeId = p.Get<int>("@MakeId"); // get output param
            }
        }
    }
}
