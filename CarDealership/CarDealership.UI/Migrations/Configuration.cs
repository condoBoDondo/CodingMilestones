namespace CarDealership.UI.Migrations
{
    using CarDealership.UI.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CarDealership.UI.Models.CarDealershipDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CarDealership.UI.Models.CarDealershipDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            // load user and role managers
            var userMgr = new UserManager<AppUser>(new UserStore<AppUser>(context));
            var roleMgr = new RoleManager<AppRole>(new RoleStore<AppRole>(context));

            // admin role
            if (!roleMgr.RoleExists("admin")) // create role if it doesn't exist
            {
                roleMgr.Create(new AppRole() { Name = "admin" }); // create admin role
                var user = new AppUser() { UserName = "admin", FirstName = "Conner", LastName = "Jensen", Email = "conner@something.com" }; // create default user
                userMgr.Create(user, "testing123"); // create user login, with password "testing123"
                userMgr.AddToRole(user.Id, "admin"); // add user to admin role
            }

            // sales role
            if (!roleMgr.RoleExists("sales"))
            {
                roleMgr.Create(new AppRole() { Name = "sales" });
                var user = new AppUser() { UserName = "sales", FirstName = "Guy", LastName = "Loadsamoney", Email = "moneybags@cash.com" };
                userMgr.Create(user, "testing123");
                userMgr.AddToRole(user.Id, "sales");
            }

            // disabled role
            if (!roleMgr.RoleExists("disabled"))
                roleMgr.Create(new AppRole() { Name = "disabled" });
        }
    }
}
