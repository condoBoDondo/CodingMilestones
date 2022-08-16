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
    public class ContactRepositoryProd : IContactRepository
    {
        // using dapper
        public void Insert(Contact contact)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                // get all params required for sproc
                // ContactId is an output param (returned to user after sproc runs)
                DynamicParameters p = new DynamicParameters();
                p.Add("@ContactId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@Name", contact.Name);
                p.Add("@Phone", contact.Phone);
                p.Add("@Email", contact.Email);
                p.Add("@Message", contact.Message);
                p.Add("@Regarding", contact.Regarding);

                // execute given command, with params, of given command type
                cn.Execute("ContactInsert", p, commandType: CommandType.StoredProcedure);

                // set returned ContactId
                contact.ContactId = p.Get<int>("@ContactId");
            }
        }
    }
}
