using CarDealership.Data.Interfaces;
using CarDealership.Models.Queries;
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
    public class UserRepositoryProd : IUserRepository
    {
        public IEnumerable<UserShort> GetAll()
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                return cn.Query<UserShort>("UserSelectShort",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public UserShort GetById(string userId)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@UserId", userId);

                return cn.Query<UserShort>("UserSelectShortById", p,
                    commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
        }

        public void Update(UserShort user)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Settings.GetConnectionString();

                DynamicParameters p = new DynamicParameters();
                p.Add("@UserId", user.Id);
                p.Add("@FirstName", user.FirstName);
                p.Add("@LastName", user.LastName);
                p.Add("@Email", user.Email);
                p.Add("@UserName", user.Email); // email matches username

                cn.Execute("UserUpdate", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
