using CarDealership.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Interfaces
{
    public interface IUserRoleRepository
    {
        IEnumerable<UserRoleShort> GetAll();
    }
}
