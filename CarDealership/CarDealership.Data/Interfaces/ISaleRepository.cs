using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Interfaces
{
    public interface ISaleRepository
    {
        IEnumerable<Sale> GetAll();
        IEnumerable<SaleReport> GetReports(SaleReportSearchParameters search); // get sales formatted properly to use in admin viewmodel
        void Purchase(Sale sale); // batched with a vehicle table update in sproc
    }
}
