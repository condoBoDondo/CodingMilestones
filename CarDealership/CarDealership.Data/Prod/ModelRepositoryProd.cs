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
    public class ModelRepositoryProd : IModelRepository
    {
        public IEnumerable<Model> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<Model>("ModelSelectAll",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<ModelByMakeId> GetByMakeId(int makeId)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                // need to set MakeId input param
                DynamicParameters p = new DynamicParameters();
                p.Add("@MakeId", makeId);

                // remember to put parameters in query!
                return cn.Query<ModelByMakeId>("ModelSelectByMake", p,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<ModelReport> GetReports()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<ModelReport>("ModelSelectAllReport",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public void Insert(Model model)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                // set up output and input params
                DynamicParameters p = new DynamicParameters();
                p.Add("@ModelId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@MakeId", model.MakeId);
                p.Add("@ModelName", model.ModelName);
                p.Add("@AddedBy", model.AddedBy);

                // execute command, with these params, of this type
                cn.Execute("ModelInsert", p, commandType: CommandType.StoredProcedure);

                model.ModelId = p.Get<int>("@ModelId"); // get output param
            }
        }
    }
}
