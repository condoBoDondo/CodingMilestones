using CarDealership.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarDealership.UI.App_Start
{
    // IdentityConfig: explicitly telling ASP.NET which components we're using
    public class IdentityConfig
    {
        // OWIN services will call this method when the website starts
        public void Configuration(IAppBuilder app)
        {
            // in OWIN context, setting classes to be used for specific purposes

            // used for data storage and retrieval
            app.CreatePerOwinContext(() => new CarDealershipDbContext());

            // contains members used for user data
            app.CreatePerOwinContext<UserManager<AppUser>>((options, context) =>
                new UserManager<AppUser>(
                    new UserStore<AppUser>(context.Get<CarDealershipDbContext>())));

            // contains members used for role data
            app.CreatePerOwinContext<RoleManager<AppRole>>((options, context) =>
                new RoleManager<AppRole>(
                    new RoleStore<AppRole>(context.Get<CarDealershipDbContext>())));

            // store authentication tokens in browser cookie;
            //   if non-authorized user attempts to use resource they have
            //   no permission to, redirect to home page
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Index")
            });
        }
    }
}