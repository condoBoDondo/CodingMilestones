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
    public class TransmissionRepositoryProd : ITransmissionRepository
    {
        public IEnumerable<Transmission> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<Transmission>("TransmissionSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
