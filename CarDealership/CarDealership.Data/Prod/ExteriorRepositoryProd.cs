﻿using CarDealership.Data.Interfaces;
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
    public class ExteriorRepositoryProd : IExteriorRepository
    {
        public IEnumerable<Exterior> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<Exterior>("ExteriorSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<ExteriorColors> GetAllWithColorNames()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<ExteriorColors>("ExteriorSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
