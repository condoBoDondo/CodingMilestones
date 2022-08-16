using CarDealership.Data.Interfaces;
using CarDealership.Data.Prod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Factories
{
    public static class ExteriorRepositoryFactory
    {
        public static IExteriorRepository GetRepository()
        {
            switch (Settings.GetMode())
            {
                case "prod":
                    return new ExteriorRepositoryProd();
                default:
                    throw new Exception("Could not find valid 'mode' config value.");
            }
        }
    }
}
