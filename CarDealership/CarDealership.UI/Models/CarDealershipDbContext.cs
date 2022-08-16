using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarDealership.UI.Models
{
    public class CarDealershipDbContext : IdentityDbContext<AppUser>
    {
        // manages DbSet properties
        // IdentityDbContext comes with pre-built DbSet props

        public CarDealershipDbContext() : base("DefaultConnection")
        {

        }
    }
}