using CarDealership.Data.Interfaces;
using CarDealership.Data.Prod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Factories
{
    public static class BodyStyleRepositoryFactory
    {
        public static IBodyStyleRepository GetRepository()
        {
            switch (Settings.GetMode()) // get web.config mode
            {
                case "prod":
                    return new BodyStyleRepositoryProd();
                default:
                    throw new Exception("Could not find valid 'mode' config value.");
            }
        }
    }
}
