using CarDealership.Data.Interfaces;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Prod
{
    public class BodyStyleRepositoryProd : IBodyStyleRepository
    {
        // using ADO
        public IEnumerable<BodyStyle> GetAll()
        {
            List<BodyStyle> styles = new List<BodyStyle>();

            // with the connection to database...
            using (var cn = new SqlConnection(Settings.GetConnectionString()))
            {
                // ... set up stored procedure command
                SqlCommand cmd = new SqlCommand("BodyStyleSelectAll", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cn.Open();

                using(SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read()) // read data rows one at a time
                    {
                        BodyStyle row = new BodyStyle();

                        row.BodyStyleId = (int)dr["BodyStyleId"];
                        row.BodyStyleName = dr["BodyStyleName"].ToString();
                        styles.Add(row);
                    }
                }
            }

            return styles;
        }
    }
}
