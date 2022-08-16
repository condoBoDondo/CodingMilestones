using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarDealership.UI.Models
{
    public class AppUser : IdentityUser
    {
        // adding custom fields
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}