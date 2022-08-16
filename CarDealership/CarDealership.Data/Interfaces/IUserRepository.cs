using CarDealership.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<UserShort> GetAll();
        UserShort GetById(string userId); // asp.net user ids are long strings
        void Update(UserShort user); // updates only info that should be modified thru sproc
    }
}
