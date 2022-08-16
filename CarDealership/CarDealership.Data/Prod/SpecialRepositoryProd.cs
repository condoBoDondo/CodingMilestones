using CarDealership.Data.Interfaces;
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
    public class SpecialRepositoryProd : ISpecialRepository
    {
        public IEnumerable<Special> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<Special>("SpecialSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public void Insert(Special special)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@SpecialId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@Title", special.Title);
                p.Add("@Description", special.Description);

                cn.Execute("SpecialInsert", p, commandType: CommandType.StoredProcedure);

                special.SpecialId = p.Get<int>("@SpecialId");
            }
        }

        public void Delete(int specialId)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@SpecialId", specialId);

                cn.Execute("SpecialDelete", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
