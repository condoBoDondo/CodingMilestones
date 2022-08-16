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
    public class InteriorRepositoryProd : IInteriorRepository
    {
        public IEnumerable<Interior> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<Interior>("InteriorSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<InteriorColors> GetAllWithColorNames()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<InteriorColors>("InteriorSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
