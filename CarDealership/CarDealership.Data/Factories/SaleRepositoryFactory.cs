using CarDealership.Data.Interfaces;
using CarDealership.Data.Prod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Factories
{
    public static class SaleRepositoryFactory
    {
        public static ISaleRepository GetRepository()
        {
            switch (Settings.GetMode())
            {
                case "prod":
                    return new SaleRepositoryProd();
                default:
                    throw new Exception("Could not find valid 'mode' config value.");
            }
        }
    }
}
